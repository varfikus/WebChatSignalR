'use strict';

var Chat = {
    el: {
        connection: new signalR.HubConnectionBuilder()
            .withUrl('/chatHub')
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build(),
        RoomId: document.getElementById('conversationInput'),
        messagesList: document.getElementById('messagesList'),
        sendButton: document.getElementById('sendButton'),
        messageInput: document.getElementById('messageInput'),
        fileInput: document.getElementById('fileInput'),
        senderId: document.getElementById('Conversation_Sender_Id'),
        senderName: document.getElementById('Conversation_Sender_Name'),
        senderAvatar: document.getElementById('Conversation_Sender_Avatar'),
        recipientName: document.getElementById('Conversation_Recipient_Name'),
        recipientAvatar: document.getElementById('Conversation_Recipient_Avatar'),
        activeUserOnlist: document.querySelector('.user-link.active'),
        UserId: document.getElementById('userInput'),
        excerpt: document.querySelector('.user-link.active .message'),
        emojiLinks: document.getElementsByClassName('emoji-link'),
        closeChatbox: document.getElementById('close-chat'),
        recordButton: document.getElementById('recordButton'),
        audioPreview: document.getElementById('audioPreview')
    },

    init: function () {
        let con = Chat.el.connection;

        con.on("ReceiveMessage", (UserId, message, file, FileName, timestamp) => {
            console.log("Received message data:", { UserId, message, file, FileName, timestamp });
            Chat.renderMessage(UserId, message, file, FileName, timestamp, function (li) {
                Chat.el.messagesList.appendChild(li);
            });
        });

        con.on("ReceiveVoiceMessage", (UserId, FilePath, timestamp) => {
            console.log("Received voice message:", { UserId, FilePath, timestamp });
            Chat.receiveVoiceMessage(UserId, FilePath, timestamp);
        });

        con.on('onError', function (message) {
            console.log(message);
        });

        con.on('Notification', function (RoomId, message) {
            Chat.showNotification(RoomId, message);
        });

        con
            .start()
            .then(function () {
                con.invoke('JoinRoom', Chat.el.RoomId.value).catch(function (err) {
                    return console.error(err.toString());
                });
            })
            .catch(function (err) {
                return console.error(err.toString());
            });

        Chat.el.sendButton.addEventListener('click', function (event) {
            Chat.sendMessage();
            event.preventDefault();
        });

        Chat.el.messageInput.addEventListener('input', function (event) {
            let unread = Chat.el.activeUserOnlist.querySelector('.unread-count');
            if (unread) {
                Chat.readMessage();
            }
            event.preventDefault();
        });

        Chat.el.messageInput.addEventListener('keydown', function (event) {
            if (event.keyCode === 13 && event.ctrlKey) {
                const textarea = Chat.el.messageInput;
                textarea.value = textarea.value + '\r\n';
            }
            if (event.key === 'Enter') {
                event.preventDefault();
                Chat.sendMessage();
            }
        });
        
        if (Chat.el.recordButton) {
            Chat.el.recordButton.addEventListener("click", async function () {
                if (!Chat.mediaRecorder || Chat.mediaRecorder.state === "inactive") {
                    Chat.startRecording();
                } else {
                    Chat.stopRecording();
                }
            });
        }

        Chat.el.closeChatbox.addEventListener('click', function (event) {
            Chat.hideChatBox();
            event.preventDefault();
        });

        Chat.el.activeUserOnlist.addEventListener('click', function (event) {
            Chat.hideUserListOnMobile();
            event.preventDefault();
        });

        Chat.hideUserListOnMobile();
        Chat.loadHistoryMessage();
        Chat.renderEmoji();
        Chat.blockUserDropdown();
        Chat.UiEmoji();
    },

    receiveVoiceMessage: function (UserId, FilePath, timestamp) {
        let time = timeago.format(new Date(timestamp));
        let name = UserId === +Chat.el.senderId.value ? Chat.el.senderName.value : Chat.el.recipientName.value;
        let avatar = UserId === +Chat.el.senderId.value ? Chat.el.senderAvatar.value : Chat.el.recipientAvatar.value;

        let voiceMessageHtml = `
    <li>
        <div class="media p-2">
            <div class="profile-avatar mr-2">
                <img class="avatar-img" src="/images/${avatar}" alt="avatar">
            </div>
            <div class="media-body overflow-hidden">
                <div class="d-flex mb-1">
                    <h6 class="text-truncate mb-0 mr-auto">${name}</h6>
                    <p class="small text-muted text-nowrap ml-4">${time}</p>
                </div>
                <div class="mt-2">
                    <audio controls>
                        <source src="${FilePath}" type="audio/webm">
                        Ваш браузер не поддерживает аудио.
                    </audio>
                </div>
            </div>
        </div>
    </li>
    `;

        let li = document.createElement("li");
        li.innerHTML = voiceMessageHtml;
        Chat.el.messagesList.appendChild(li);
    },

    sendMessage: function () {
        const UserId = Chat.el.UserId.value;
        const message = Chat.el.messageInput.value.trim();
        const conversationId = Chat.el.RoomId.value;
        const fileInput = Chat.el.fileInput.files[0];

        if (!message && !fileInput) return;

        if (fileInput) {
            const reader = new FileReader();
            reader.readAsArrayBuffer(fileInput);
            reader.onloadend = function () {
                const arrayBuffer = reader.result;
                const bytes = new Uint8Array(arrayBuffer);

                const binary = bytes.reduce((acc, byte) => acc + String.fromCharCode(byte), "");
                const base64String = btoa(binary);

                Chat.el.connection
                    .invoke('SendMessage', conversationId, UserId, message || null, base64String, fileInput.name)
                    .then(() => {
                        Chat.el.excerpt.innerHTML = Chat.BasicEmojis.parseEmojis(message);
                    })
                    .catch(err => console.error(err.toString()));
            };
        } else {
            Chat.el.connection
                .invoke('SendMessage', conversationId, UserId, message || null, null, null)
                .then(() => {
                    Chat.el.excerpt.innerHTML = Chat.BasicEmojis.parseEmojis(message);
                })
                .catch(err => console.error(err.toString()));
        }

        Chat.el.messageInput.value = '';
        Chat.el.fileInput.value = '';
    },
    startRecording: async function () {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            Chat.mediaRecorder = new MediaRecorder(stream);
            Chat.audioChunks = [];

            Chat.mediaRecorder.ondataavailable = event => {
                if (event.data.size > 0) {
                    Chat.audioChunks.push(event.data);
                }
            };

            Chat.mediaRecorder.onstop = async () => {
                const audioBlob = new Blob(Chat.audioChunks, { type: "audio/webm" });
                Chat.el.audioPreview.src = URL.createObjectURL(audioBlob);
                Chat.el.audioPreview.classList.remove("d-none");

                await Chat.uploadAudio(audioBlob);
            };

            Chat.mediaRecorder.start();
            Chat.el.recordButton.textContent = "⏹️";
        } catch (error) {
            console.error("Ошибка доступа к микрофону", error);
        }
    },

    stopRecording: function () {
        if (Chat.mediaRecorder && Chat.mediaRecorder.state !== "inactive") {
            Chat.mediaRecorder.stop();
            Chat.el.recordButton.textContent = "🎤";
        }
    },

    uploadAudio: async function (blob) {
        console.log("Размер файла перед отправкой:", blob.size); 

        const formData = new FormData();
        formData.append("audio", blob, "voice-message.webm");
        formData.append("RoomId", Chat.el.RoomId.value);
        formData.append("UserId", Chat.el.UserId.value);

        try {
            debugger;
            const response = await fetch("/Chat/UploadVoiceMessage", {
                method: "POST",
                body: formData
            });

            if (response.ok) {
                console.log("Аудиофайл отправлен успешно");
                Chat.el.audioPreview.src = "";
                Chat.el.audioPreview.classList.add("d-none");
                Chat.el.recordButton.textContent = "🎤";
            } else {
                console.error("Ошибка при отправке аудио", await response.text()); 
            }
        } catch (error) {
            console.error("Ошибка сети", error);
        }
    },

    readMessage: function () {
        let unread = document
            .querySelector('.user-link.active')
            .querySelector('.unread-count')
        if (unread) {
            const options = {
                method: 'PUT',
                body: Chat.el.RoomId.value,
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
            Ajax.makeRequest('/Chat/ReadMessage', options)
                .then((data) => {
                    if (data.response) {
                        unread.remove()
                    }
                })
                .catch((error) => {
                    console.log(error)
                })
        }
    },
    renderMessage: function (UserId, message, file, FileName, timestamp, callback) {
        let time = timeago.format(new Date(timestamp));
        let name, avatar;

        if (UserId === +Chat.el.senderId.value) {
            name = Chat.el.senderName.value;
            avatar = Chat.el.senderAvatar.value;
        } else {
            name = Chat.el.recipientName.value;
            avatar = Chat.el.recipientAvatar.value;
        }

        let parsedMessage = message ? Chat.BasicEmojis.parseEmojis(message) : "";
        let fileHtml = "";

        if (file && file.length > 0) {
            let mimeType = "application/octet-stream";

            if (FileName) {
                if (FileName.match(/\.(jpg|jpeg|png|gif|bmp)$/i)) {
                    mimeType = "image/png";
                } else if (FileName.match(/\.(pdf)$/i)) {
                    mimeType = "application/pdf";
                } else if (FileName.match(/\.(doc|docx)$/i)) {
                    mimeType = "application/msword";
                }
            }

            if (mimeType.startsWith("image")) {
                fileHtml = `
                <div class="mt-2">
                    <img src="data:${mimeType};base64,${file}" alt="Image" class="img-fluid" style="max-width: 200px; border-radius: 8px;">
                </div>
                `;
            } else {
                fileHtml = `
                <div class="mt-2">
                    <a href="data:${mimeType};base64,${file}" download="${FileName || 'file'}">
                        Скачать вложение (${FileName || "файл"})
                    </a>
                </div>
                `;
            }
        }

        if (!parsedMessage && fileHtml) {
            parsedMessage = "<i>(Файл)</i>";
        }

        let encodedMsg = `
        <li>
            <div class="media p-2">
                <div class="profile-avatar mr-2">
                    <img class="avatar-img" src="/images/${avatar}" alt="avatar">
                </div>
                <div class="media-body overflow-hidden">
                    <div class="d-flex mb-1">
                        <h6 class="text-truncate mb-0 mr-auto">${name}</h6>
                        <p class="small text-muted text-nowrap ml-4">${time}</p>
                    </div>
                    <div class="text-wrap text-break p-1">${parsedMessage}</div>
                    ${fileHtml}
                </div>
            </div>
        </li>
        `;

        let li = document.createElement("li");
        li.innerHTML = encodedMsg;
        callback(li);
    },

  renderEmoji: function () {
    window.onload = function () {
      let messages = document.getElementsByClassName('message')
      for (let message of messages) {
        message.innerHTML = Chat.BasicEmojis.parseEmojis(message.innerText)
      }
    }
  },
  UiEmoji: function () {
    const emojiLinks = Chat.el.emojiLinks
    for (let item of emojiLinks) {
      item.addEventListener('click', function (event) {
        event.preventDefault()
        let input = Chat.el.messageInput
        if (input.selectionStart || input.selectionStart === 0) {
          Chat.el.messageInput.value = [
            input.value.slice(0, input.selectionStart),
            item.firstElementChild.value,
            input.value.slice(input.selectionEnd),
          ].join('')
        }
      })
    }
  },
  ExcerptEmoji: function () {
    console.log(Chat.el.excerpt.textContent)
    if (Chat.el.excerpt.textContent) {
      Chat.el.excerpt.innerHTML = Chat.BasicEmojis.parseEmojis(
        Chat.el.excerpt.textContent
      )
    }
  },
  BasicEmojis: {
    parseEmojis: function (content) {
      content = content.replace(/(:\))/g, this.addImage('emoji1.png'))
      content = content.replace(/(:[P])/g, this.addImage('emoji2.png'))
      content = content.replace(/(:[O])/g, this.addImage('emoji3.png'))
      content = content.replace(/(:[\-]+\))/g, this.addImage('emoji4.png'))
      content = content.replace(/(B\|)/g, this.addImage('emoji5.png'))
      content = content.replace(/(:[D])/g, this.addImage('emoji6.png'))
      content = content.replace(/(<3)/g, this.addImage('emoji7.png'))
      return content
    },
    addImage: function (imageName) {
      return `<img class="emoji" alt="emoji" src="/images/emojis/${imageName}">`
    },
  },
  showNotification: function (RoomId, message) {
    let roomEl = document.querySelector("li[data-RoomId='" + RoomId + "']")
    if (roomEl) {
      let unread = roomEl.querySelector('.unread-count')
      if (unread) {
        let count = parseInt(unread.textContent)
        count++
        unread.textContent = count
        roomEl.querySelector('.last-msg').innerText = message
        roomEl.querySelector('.last-updated').innerText = window.timeago(
          new Date() - new Date()
        )
        Chat.ExcerptEmoji()
      } else {
        roomEl.querySelector('.last-msg').innerText = message
        roomEl.querySelector('.last-updated').innerText = window.timeago(
          new Date() - new Date()
        )
        let elStr =
          '<div class="badge badge-circle badge-primary badge-border-light badge-top-right unread-count"><span>1</span></div>'
        let div = document.createElement('div')
        div.innerHTML = elStr
        roomEl.appendChild(div)
        Chat.ExcerptEmoji()
      }
    } else {
      Chat.newShowNotification(RoomId, message)
    }
  },
  newShowNotification: function (RoomId, message) {
    const options = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
    }
    let url = '/Chat/GetChatUser/' + RoomId
    Ajax.makeRequest(url, options).then((data) => {
      console.log(data.response)
      if (data.response) {
        let { id, name, avatar } = data.response
        let notification = `<li class="list-group-item list-group-item-action user-link " data-RoomId="${RoomId}" data-UserId="${id}">
                               <a class="position-absolute inset-0 " href="/Chat/${id}"> </a>
                               <div class="media">
                                   <div class="profile-avatar mr-2">
                                       <img class="avatar-img" src="~/images/${avatar}" >
                                   </div>
                                   <div class="media-body overflow-hidden">
                                       <div class="d-flex mb-1">
                                           <h6 class="text-truncate mb-0 mr-auto username">${name}</h6>
                                           <p class="small text-nowrap last-updated" datetime="${new Date()}">${window.timeago(
          new Date() - new Date()
        )}</p>
                                       </div>
                                       <div class="text-truncate last-msg p-1 message">${message}</div>
                                   </div>
                               </div>
                                <div class="badge badge-circle badge-primary badge-border-light badge-top-right unread-count"><span>1</span></div>
                           </li>`
        console.log(notification)
        document
          .querySelector('.user-list > .list-group')
          .insertAdjacentHTML('afterbegin', notification)
      }
    })
    Chat.ExcerptEmoji()
  },
  loadHistoryMessage: function () {
    let timer
    document.getElementsByClassName('chat-box')[0].addEventListener(
      'scroll',
      function () {
        const load = document.getElementById('load-more')
        if (timer) {
          clearTimeout(timer)
        }
        timer = setTimeout(() => {
          if (Utils.isInViewPort(load)) {
            // // update the element display
            // console.log('in viewport')
            const currentPage = document.getElementById(
              'Conversation_Messages_CurrentPage'
            ).value
            const pageCount = document.getElementById(
              'Conversation_Messages_PageCount'
            ).value
            const conversationId =
              document.getElementById('conversationInput').value
            if (currentPage !== pageCount) {
              const url = `${
                window.location.origin
              }/Chat/LoadHistory/${conversationId}?page=${+currentPage + 1}`
              const options = {
                method: 'Get',
                headers: {
                  Accept: 'application/json',
                  'Content-Type': 'application/json',
                },
              }
              Ajax.makeRequest(url, options)
                .then((data) => {
                  if (data.response || data.response.results.length > 0) {
                    data.response.results.forEach((x) => {
                      renderMessage(
                        x.senderId,
                        x.content,
                        x.timestamp,
                        function (li) {
                          insertAfter(li, document.getElementById('load-more'))
                        }
                      )
                    })
                    document.getElementById(
                      'Conversation_Messages_CurrentPage'
                    ).value = data.response.currentPage
                  }
                })
                .catch((error) => {
                  console.log(error)
                })
            }
          }
        }, 1000)
      },
      false
    )
  },
  blockUser: function (el, status) {
    const RoomId = document.getElementById('conversationInput').value
    let url =
      status === 'block'
        ? '/Chat/BlockUser/' + RoomId
        : '/Chat/BlockUser/' + RoomId + '?IsReported=true'
    const options = {
      method: 'POST',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
    }
    Ajax.makeRequest(url, options)
      .then((data) => {
        if (data.response) {
          if (data.response.success) {
            document.getElementById('sendButton').disabled = false
            document.getElementById('messageInput').disabled = false
            document.getElementById('chatIcon').disabled = false
            document.getElementById('chat-blocker').classList.toggle('d-none')
            document
              .getElementById('chat-box')
              .classList.replace('overflow-hidden', 'overflow-auto')
            if (status === 'block') {
              el.textContent = '🚫 Unblock User'
            } else {
              el.textContent = '☣️ Remove Report'
            }
          }
        }
      })
      .catch((error) => {
        console.log(error)
      })
  },
  blockUserDropdown: function () {
    const chatDropLinks = document.querySelectorAll('#chat-dropdown a')
    for (let link of chatDropLinks) {
      link.addEventListener('click', function (e) {
        e.preventDefault() // cancel the link behaviour
        const el = e.target
        el.textContent === '🚫 Block User'
          ? Chat.blockUser(el, 'block')
          : Chat.blockUser(el, 'report')
      })
    }
  },
  isMoblile: function () {
    return window.matchMedia('only screen and (max-width: 760px)').matches
  },
  hideUserListOnMobile: function () {
    if (this.isMoblile && window.location.pathname.split('/')[2].length) {
      document
        .getElementsByClassName('chat-area')[0]
        .classList.add('chat-visible')
    } else {
      document
        .getElementsByClassName('chat-area')[0]
        .classList.remove('chat-visible')
    }
  },
  hideChatBox: function () {
    document
      .getElementsByClassName('chat-area')[0]
      .classList.remove('chat-visible')
  },
}

Chat.init()

const Ajax = {
  makeRequest: function (url, options) {
    return new Promise((resolve, reject) => {
      fetch(url, options)
        .then(this.handleResponse)
        .then((response) => JSON.parse(response))
        .then((json) => resolve(json))
        .catch(this.handleError)
        .catch((error) => {
          try {
            reject(JSON.parse(error))
          } catch (e) {
            reject(error)
          }
        })
    })
  },
  handleResponse: function (response) {
    return response.json().then((json) => {
      // Modify response to include status ok, success, and status text
      let modifiedJson = {
        success: response.ok,
        status: response.status,
        statusText: response.statusText
          ? response.statusText
          : json.error || '',
        response: json,
      }
      // If request failed, reject and return modified json string as error
      if (!modifiedJson.success)
        return Promise.reject(JSON.stringify(modifiedJson))
      // If successful, continue by returning modified json string
      return JSON.stringify(modifiedJson)
    })
  },
  handleError: function (errorRes) {
    const errorResponse = JSON.parse(JSON.stringify(errorRes))
    const responseError = {
      type: 'Error',
      message:
        errorResponse.message ||
        errorResponse.response.message ||
        'Something went wrong',
      data: errorResponse.response || '',
      code: errorResponse.status || '',
    }
    let error = new Error()
    error = { ...error, ...responseError }
    console.log(error)
    throw error
  },
}

var Utils = {
  isInViewPort: function (element) {
    // Get the bounding client rectangle position in the viewport
    const bounding = element.getBoundingClientRect()

    // Checking part. Here the code checks if it's *fully* visible
    // Edit this part if you just want a partial visibility
    return (
      bounding.top >= 0 &&
      bounding.left >= 0 &&
      bounding.right <=
        (window.innerWidth || document.documentElement.clientWidth) &&
      bounding.bottom <=
        (window.innerHeight || document.documentElement.clientHeight)
    )
  },

  insertAfter: function (newNode, referenceNode) {
    referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling)
  },
}

//const connection = new signalR.HubConnectionBuilder()
//  .withUrl('/chatHub')
//  .withAutomaticReconnect()
//  .configureLogging(signalR.LogLevel.Information)
//  .build()
//
//connection.on('ReceiveMessage', function (UserId, message, timestamp) {
//  renderMessage(UserId, message, timestamp, function (li) {
//    document.getElementById('messagesList').appendChild(li)
//  })
//})
//
//connection.on('onError', function (message) {
//  console.log(message)
//})
//
//function showNotification(RoomId, message) {
//  let roomEl = document.querySelector("li[data-RoomId='" + RoomId + "']")
//  if (roomEl) {
//    let unread = roomEl.querySelector('.unread-count')
//    if (unread) {
//      let count = parseInt(unread.textContent)
//      count++
//      unread.textContent = count
//      roomEl.querySelector('.last-msg').innerText = message
//      roomEl.querySelector('.last-updated').innerText = window.timeago(
//        new Date() - new Date()
//      )
//    } else {
//      console.log('msg in dom')
//      roomEl.querySelector('.last-msg').innerText = message
//      roomEl.querySelector('.last-updated').innerText = window.timeago(
//        new Date() - new Date()
//      )
//      let elStr =
//        '<div class="badge badge-circle badge-primary badge-border-light badge-top-right unread-count"><span>1</span></div>'
//      let div = document.createElement('div')
//      div.innerHTML = elStr
//      roomEl.appendChild(div)
//    }
//  } else {
//    newShowNotification(RoomId, message)
//  }
//}
//function newShowNotification(RoomId, message) {
//  const options = {
//    method: 'GET',
//    headers: {
//      Accept: 'application/json',
//      'Content-Type': 'application/json',
//    },
//  }
//  let url = '/Chat/GetChatUser/' + RoomId
//  Ajax.makeRequest(url, options).then((data) => {
//    console.log(data.response)
//    if (data.response) {
//      let { id, name, avatar } = data.response
//      let notification = `<li class="list-group-item list-group-item-action user-link text-white" data-RoomId="${RoomId}" data-UserId="${id}">
//                               <a class="position-absolute inset-0 " href="/Chat/${id}"> </a>
//                               <div class="media">
//                                   <div class="profile-avatar mr-2">
//                                       <img class="avatar-img" src="~/images/${avatar}" >
//                                   </div>
//                                   <div class="media-body overflow-hidden">
//                                       <div class="d-flex mb-1">
//                                           <h6 class="text-truncate mb-0 mr-auto username">${name}</h6>
//                                           <p class="small text-nowrap last-updated" datetime="${window.timeago(
//                                             new Date() - new Date()
//                                           )}"></p>
//                                       </div>
//                                       <div class="text-truncate last-msg p-1 message">${message}</div>
//                                   </div>
//                               </div>
//                           </li>`
//      console.log(notification)
//      document
//        .querySelector('.user-list > .list-group')
//        .insertAdjacentHTML('afterbegin', notification)
//    }
//  })
//}
//
//connection.on('Notification', function (RoomId, message) {
//  showNotification(RoomId, message)
//})
//connection
//  .start()
//  .then(function () {
//    const RoomId = document.getElementById('conversationInput').value
//    connection.invoke('JoinRoom', RoomId).catch(function (err) {
//      return console.error(err.toString())
//    })
//  })
//  .catch(function (err) {
//    return console.error(err.toString())
//  })
//
//document
//  .getElementById('sendButton')
//  .addEventListener('click', function (event) {
//    sendMessage()
//    event.preventDefault()
//  })
//document
//  .getElementById('messageInput')
//  .addEventListener('click', function (event) {
//    readMessage()
//    event.preventDefault()
//  })
//document
//    .getElementById('messageInput')
//    .addEventListener('input', function (event) {
//        readMessage()
//        event.preventDefault()
//    })
//document
//  .getElementById('messageInput')
//  .addEventListener('keydown', function (event) {
//    if (event.keyCode === 13 && event.ctrlKey) {
//      const textarea = document.querySelector('#messageInput')
//      textarea.value = textarea.value + '\r\n'
//    }
//    if (event.key === 'Enter') {
//      event.preventDefault()
//      // Do more work
//      sendMessage()
//    }
//  })
//const emojiLinks = document.getElementsByClassName('emoji-link')
//for (let item of emojiLinks) {
//  item.addEventListener('click', function (event) {
//    event.preventDefault()
//    let input = document.getElementById('messageInput')
//    if (input.selectionStart || input.selectionStart === 0) {
//      document.getElementById('messageInput').value = [
//        input.value.slice(0, input.selectionStart),
//        item.firstElementChild.value,
//        input.value.slice(input.selectionEnd),
//      ].join('')
//    }
//  })
//}
//
//function sendMessage() {
//  const UserId = document.getElementById('userInput').value
//  let message = document.getElementById('messageInput').value
//  const conversationId = document.getElementById('conversationInput').value
//  message = message.trim()
//  if (message) {
//    connection
//      .invoke('SendMessage', conversationId, UserId, message)
//      .then(function () {
//        document.querySelector(
//          '.user-link.active .message'
//        ).textContent = message
//      })
//      .catch(function (err) {
//        return console.error(err.toString())
//      })
//  }
//  document.getElementById('messageInput').value = ''
//}
//
//function readMessage() {
//  let unread = document
//    .querySelector('.user-link.active')
//    .querySelector('.unread-count')
//  if (unread) {
//    const conversationId = document.getElementById('conversationInput').value
//    const options = {
//      method: 'PUT',
//      body: conversationId,
//      headers: {
//        Accept: 'application/json',
//        'Content-Type': 'application/json',
//      },
//    }
//    Ajax.makeRequest('/Chat/ReadMessage', options)
//      .then((data) => {
//        if (data.response) {
//          unread.remove()
//        }
//      })
//      .catch((error) => {
//        console.log(error)
//      })
//  }
//}
//
//let timer
//document.getElementsByClassName('chat-box')[0].addEventListener(
//  'scroll',
//  function () {
//    const load = document.getElementById('load-more')
//    if (timer) {
//      clearTimeout(timer)
//    }
//    timer = setTimeout(() => {
//      if (isInViewPort(load)) {
//         // update the element display
//         console.log('in viewport')
//        const currentPage = document.getElementById(
//          'Conversation_Messages_CurrentPage'
//        ).value
//        const pageCount = document.getElementById(
//          'Conversation_Messages_PageCount'
//        ).value
//        const conversationId = document.getElementById('conversationInput')
//          .value
//        if (currentPage !== pageCount) {
//          const url = `${
//            window.location.origin
//          }/Chat/LoadHistory/${conversationId}?page=${+currentPage + 1}`
//          const options = {
//            method: 'Get',
//            headers: {
//              Accept: 'application/json',
//              'Content-Type': 'application/json',
//            },
//          }
//          Ajax.makeRequest(url, options)
//            .then((data) => {
//              if (data.response || data.response.results.length > 0) {
//                data.response.results.forEach((x) => {
//                  renderMessage(
//                    x.senderId,
//                    x.content,
//                    x.timestamp,
//                    function (li) {
//                      insertAfter(li, document.getElementById('load-more'))
//                    }
//                  )
//                })
//                document.getElementById(
//                  'Conversation_Messages_CurrentPage'
//                ).value = data.response.currentPage
//              }
//            })
//            .catch((error) => {
//              console.log(error)
//            })
//        }
//      }
//    }, 1000)
//  },
//  false
//)
//
//function renderMessage(UserId, message, timestamp, callback) {
//  let time = window.timeago(new Date() - new Date(timestamp))
//  let name, avatar
//  if (UserId === +document.getElementById('Conversation_Sender_Id').value) {
//    name = document.getElementById('Conversation_Sender_Name').value
//    avatar = document.getElementById('Conversation_Sender_Avatar').value
//  } else {
//    name = document.getElementById('Conversation_Recipient_Name').value
//    avatar = document.getElementById('Conversation_Recipient_Avatar').value
//  }
//  let parsedMessage = BasicEmojis.parseEmojis(message)
//  let encodedMsg = `<li>
//                        <div class="media p-2">
//                            <div class="profile-avatar mr-2">
//                                <img class="avatar-img" src="~/images/${avatar}" >
//                            </div>
//
//                            <div class="media-body overflow-hidden">
//                                <div class="d-flex mb-1">
//                                    <h6 class="text-truncate mb-0 mr-auto">${name}</h6>
//                                    <p class="small text-muted text-nowrap ml-4">${time}</p>
//                                </div>
//                                <div class="text-wrap text-break p-1" >${parsedMessage}</div>
//                            </div>
//                        </div>
//                    </li>`
//  let li = document.createElement('li')
//  li.innerHTML = encodedMsg
//  callback(li)
//}

//function isInViewPort(element) {
//  // Get the bounding client rectangle position in the viewport
//  const bounding = element.getBoundingClientRect()
//
//  // Checking part. Here the code checks if it's *fully* visible
//  // Edit this part if you just want a partial visibility
//  return (
//    bounding.top >= 0 &&
//    bounding.left >= 0 &&
//    bounding.right <=
//      (window.innerWidth || document.documentElement.clientWidth) &&
//    bounding.bottom <=
//      (window.innerHeight || document.documentElement.clientHeight)
//  )
//}
//
//function insertAfter(newNode, referenceNode) {
//  referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling)
//}
//
//const BasicEmojis = {
//  parseEmojis: function (content) {
//    // content = `<span>${content}</span>`;
//    content = content.replace(/(:\))/g, this.addImage('emoji1.png'))
//    // content = content.replace(":)", this.addImage("emoji1.png"));
//    content = content.replace(/(:[P])/g, this.addImage('emoji2.png'))
//    // content = content.replace(":P", this.addImage("emoji2.png"));
//    content = content.replace(/(:[O])/g, this.addImage('emoji3.png'))
//    // content = content.replace(":O", this.addImage("emoji3.png"));
//    content = content.replace(/(:[\-]+\))/g, this.addImage('emoji4.png'))
//    // content = content.replace(":-)", this.addImage("emoji4.png"));
//    content = content.replace(/(B\|)/g, this.addImage('emoji5.png'))
//    // content = content.replace("B|", this.addImage("emoji5.png"));
//    content = content.replace(/(:[D])/g, this.addImage('emoji6.png'))
//    // content = content.replace(":D", this.addImage("emoji6.png"));
//    content = content.replace(/(<3)/g, this.addImage('emoji7.png'))
//    // content = content.replace("<3", this.addImage("emoji7.png"));
//    return content
//  },
//  addImage: function (imageName) {
//    return `<img class="emoji" alt="emoji" src="~/images/emojis/${imageName}">`
//  },
//}
//
//window.onload = function () {
//  let messages = document.getElementsByClassName('message')
//  for (let message of messages) {
//    message.innerHTML = BasicEmojis.parseEmojis(message.innerText)
//  }
//}
//
//const chatDropLinks = document.querySelectorAll('#chat-dropdown a')
//for (let link of chatDropLinks) {
//  link.addEventListener('click', function (e) {
//    e.preventDefault() // cancel the link behaviour
//    const el = e.target
//    el.textContent === '🚫 Block User'
//      ? blockUser(el, 'block')
//      : blockUser(el, 'report')
//  })
//}
//
//function blockUser(el, status) {
//  const RoomId = document.getElementById('conversationInput').value
//  let url =
//    status === 'block'
//      ? '/Chat/BlockUser/' + RoomId
//      : '/Chat/BlockUser/' + RoomId + '?IsReported=true'
//  const options = {
//    method: 'POST',
//    headers: {
//      Accept: 'application/json',
//      'Content-Type': 'application/json',
//    },
//  }
//  Ajax.makeRequest(url, options)
//    .then((data) => {
//      if (data.response) {
//        if (data.response.success) {
//          document.getElementById('sendButton').disabled = false
//          document.getElementById('messageInput').disabled = false
//          document.getElementById('chatIcon').disabled = false
//          document.getElementById('chat-blocker').classList.toggle('d-none')
//          document
//            .getElementById('chat-box')
//            .classList.replace('overflow-hidden', 'overflow-auto')
//          if (status === 'block') {
//            el.textContent = '🚫 Unblock User'
//          } else {
//            el.textContent = '☣️ Remove Report'
//          }
//        }
//      }
//    })
//    .catch((error) => {
//      console.log(error)
//    })
//}
