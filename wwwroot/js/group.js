'use strict';

var Group = {
    el: {
        connection: new signalR.HubConnectionBuilder()
            .withUrl('/groupHub')
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build(),
        RoomId: document.getElementById('conversationInput'),
        messagesList: document.getElementById('messagesList'),
        sendButton: document.getElementById('sendButton'),
        messageInput: document.getElementById('messageInput'),
        senderId: document.getElementById('Conversation_Sender_Id'),
        senderName: document.getElementById('Conversation_Sender_Name'),
        senderAvatar: document.getElementById('Conversation_Sender_Avatar'),
        recipientName: document.getElementById('Conversation_Recipient_Name'),
        recipientAvatar: document.getElementById('Conversation_Recipient_Avatar'),
        activeUserOnlist: document.querySelector('.user-link.active'),
        excerpt: document.querySelector('.user-link.active .message'),
        emojiLinks: document.getElementsByClassName('emoji-link'),
        closeChatbox: document.getElementById('close-chat'),
        recordButton: document.getElementById('recordButton'),
        audioPreview: document.getElementById('audioPreview'),
        fileInput: document.getElementById("fileInput"),
        GroupId: document.getElementById("GroupId").value,
        UserId: document.getElementById("UserId"),
    },

    init: function () {
        let con = Group.el.connection;

        con.on("ReceiveGroupMessage", (UserId, message, file, FileName, timestamp, GroupIdNew) => {
            Group.renderGroupMessage(UserId, message, file, FileName, timestamp, GroupIdNew, function (li) {
                Group.el.messagesList.appendChild(li);
            });
        });

        con.on("ReceiveGroupVoiceMessage", (UserId, FilePath, timestamp, GroupId) => {
            Group.receiveGroupVoiceMessage(UserId, FilePath, timestamp, GroupId);
        });

        con.on('onError', function (message) {
            console.log(message);
        });

        con.on('Notification', function (RoomId, message) {
            Group.showNotification(RoomId, message);
        });

        con
            .start()
            .then(function () {
                con.invoke('JoinGroup', Group.el.GroupId, Group.el.UserId.value).catch(function (err) {
                    return console.error(err.toString());
                });
            })
            .catch(function (err) {
                return console.error(err.toString());
            });

        Group.el.sendButton.addEventListener('click', function (event) {
            Group.sendGroupMessage();
            event.preventDefault();
        });

        Group.el.messageInput.addEventListener('input', function (event) {
            let unread = Group.el.activeUserOnlist ? Group.el.activeUserOnlist.querySelector('.unread-count') : null;
            if (unread) {
                Group.readMessage();
            }
            event.preventDefault();
        });

        Group.el.messageInput.addEventListener('keydown', function (event) {
            if (event.keyCode === 13 && event.ctrlKey) {
                const textarea = Group.el.messageInput;
                textarea.value = textarea.value + '\r\n';
            }
            if (event.key === 'Enter') {
                event.preventDefault();
                Group.sendGroupMessage();
            }
        });

        if (Group.el.recordButton) {
            Group.el.recordButton.addEventListener("click", async function () {
                if (!Group.mediaRecorder || Group.mediaRecorder.state === "inactive") {
                    Group.startRecording();
                } else {
                    Group.stopRecording();
                }
            });
        }

        Group.el.closeChatbox.addEventListener('click', function (event) {
            Group.hideChatBox();
            event.preventDefault();
        });

        Group.el.activeUserOnlist.addEventListener('click', function (event) {
            Group.hideUserListOnMobile();
            event.preventDefault();
        });

        Group.hideUserListOnMobile();
        Group.loadHistoryMessage();
        Group.renderEmoji();
        Group.blockUserDropdown();
        Group.UiEmoji();
        Group.usersCache = {};
    },

    //getUsers: async function (GroupId) {
    //    // Ensure GroupId is a number
    //    GroupId = parseInt(GroupId, 10);
    //    if (isNaN(GroupId)) {
    //        console.error("Invalid GroupId:", GroupId);
    //        return {};
    //    }

    //    try {
    //        let response = await fetch(`/Group/GetGroupUsers/${GroupId}`);
    //    if (!response.ok) throw new Error(`Failed to fetch users, status: ${response.status}`);

    //    let users = await response.json();
    //    if (!Array.isArray(users)) throw new Error("Invalid response format");

    //    users.forEach(user => {
    //        Group.usersCache[user.Id] = {
    //        name: user.Name,
    //        avatar: user.Avatar
    //        };
    //    });

    //    console.log("Users loaded:", Group.usersCache);
    //    return Group.usersCache;
    //} catch (error) {
    //    console.error("Error loading group users:", error);
    //    return {}; // Return empty object to prevent crashes
    //}
    //},


    receiveGroupVoiceMessage: async function (UserId, FilePath, timestamp, GroupId) {
        let time = timeago.format(new Date(timestamp));
        
        let users = [];
        try {
            let response = await fetch(`/Group/GetGroupUsers/${GroupId}`);
            if (!response.ok) throw new Error(`Failed to fetch users, status: ${response.status}`);

            users = await response.json();
            if (!Array.isArray(users)) throw new Error("Invalid response format");
            
        } catch (error) {
            console.error("Error fetching group users:", error);
            return;
        }
        
        let author = users.find(u => u.id == UserId) || { name: "Unknown User", avatar: "~/images/default-avatar.png" };

        let voiceMessageHtml = `
<li>
    <div class="media p-2">
        <div class="profile-avatar mr-2">
            <img class="avatar-img" src="/images/${author.avatar}" alt="avatar">
        </div>
        <div class="media-body overflow-hidden">
            <div class="d-flex mb-1">
                <h6 class="text-truncate mb-0 mr-auto">${author.name}</h6>
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
        Group.el.messagesList.appendChild(li);
    },

    sendGroupMessage: function () {
        const UserId = Group.el.UserId.value;
        const message = Group.el.messageInput.value.trim();
        const GroupId = Group.el.GroupId; 
        const fileInput = Group.el.fileInput.files[0];
        if (!message && !fileInput) return;

        if (fileInput) {
            const reader = new FileReader();
            reader.readAsArrayBuffer(fileInput);
            reader.onloadend = function () {
                const arrayBuffer = reader.result;
                const bytes = new Uint8Array(arrayBuffer);
                const binary = bytes.reduce((acc, byte) => acc + String.fromCharCode(byte), "");
                const base64String = btoa(binary);

                console.log(GroupId, UserId, message, base64String, fileInput.name)

                Group.el.connection
                    .invoke('SendGroupMessage', GroupId, UserId, message || null, base64String, fileInput.name) 
                    .then(() => {
                        if (Group.el.excerpt) {
                            Group.el.excerpt.innerHTML = Group.BasicEmojis.parseEmojis(message);
                        }
                    })
                    .catch(err => console.error(err.toString()));
            };
        } else {
            Group.el.connection
                .invoke('SendGroupMessage', GroupId, UserId, message || null, null, null) 
                .then(() => {
                    if (Group.el.excerpt) {
                        Group.el.excerpt.innerHTML = Group.BasicEmojis.parseEmojis(message);
                    }
                })
                .catch(err => console.error(err.toString()));
        }

        Group.el.messageInput.value = '';
        Group.el.fileInput.value = '';
    },
    startRecording: async function () {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            Group.mediaRecorder = new MediaRecorder(stream);
            Group.audioChunks = [];

            Group.mediaRecorder.ondataavailable = event => {
                if (event.data.size > 0) {
                    Group.audioChunks.push(event.data);
                }
            };

            Group.mediaRecorder.onstop = async () => {
                const audioBlob = new Blob(Group.audioChunks, { type: "audio/webm" });
                Group.el.audioPreview.src = URL.createObjectURL(audioBlob);
                Group.el.audioPreview.classList.remove("d-none");

                await Group.uploadAudio(audioBlob);
            };

            Group.mediaRecorder.start();
            Group.el.recordButton.textContent = "⏹️";
        } catch (error) {
            console.error("Ошибка доступа к микрофону", error);
        }
    },

    stopRecording: function () {
        if (Group.mediaRecorder && Group.mediaRecorder.state !== "inactive") {
            Group.mediaRecorder.stop();
            Group.el.recordButton.textContent = "🎤";
        }
    },

    uploadAudio: async function (blob) {
        const formData = new FormData();
        formData.append("audio", blob, "voice-message.webm");
        formData.append("GroupId", Group.el.GroupId);
        formData.append("UserId", Group.el.UserId.value);

        try {
            const response = await fetch("/Group/UploadVoiceMessage", {
                method: "POST",
                body: formData
            });

            if (response.ok) {
                Group.el.audioPreview.src = "";
                Group.el.audioPreview.classList.add("d-none");
                Group.el.recordButton.textContent = "🎤";
            } else {
                console.error("Ошибка при отправке аудио");
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
                body: Group.el.RoomId.value,
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
   
    renderGroupMessage: async function (UserId, message, file, FileName, timestamp, GroupIdNew, callback) {
        if (!GroupIdNew) {
            return;
        }

        let time = timeago.format(new Date(timestamp));

        let users = [];
        try {
            let response = await fetch(`/Group/GetGroupUsers/${GroupIdNew}`);
            if (!response.ok) throw new Error(`Failed to fetch users, status: ${response.status}`);

            users = await response.json();
            if (!Array.isArray(users)) throw new Error("Invalid response format");
            
        } catch (error) {
            console.error("Error fetching group users:", error);
            return;
        }
        
        let author = users.find(u => u.id == UserId) || { Name: "Unknown User", Avatar: "~/images/default-avatar.png" };

        let messageClass = "received";

        let parsedMessage = message ? Group.BasicEmojis.parseEmojis(message) : "";
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
<li class="${messageClass}">
    <div class="media p-2">
        <div class="profile-avatar mr-2">
            <img class="avatar-img" src="/images/${author.avatar}" alt="avatar">
        </div>
        <div class="media-body overflow-hidden">
            <div class="d-flex mb-1">
                <h6 class="text-truncate mb-0 mr-auto">${author.name}</h6>
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
                message.innerHTML = Group.BasicEmojis.parseEmojis(message.innerText)
            }
        }
    },
    UiEmoji: function () {
        const emojiLinks = Group.el.emojiLinks
        for (let item of emojiLinks) {
            item.addEventListener('click', function (event) {
                event.preventDefault()
                let input = Group.el.messageInput
                if (input.selectionStart || input.selectionStart === 0) {
                    Group.el.messageInput.value = [
                        input.value.slice(0, input.selectionStart),
                        item.firstElementChild.value,
                        input.value.slice(input.selectionEnd),
                    ].join('')
                }
            })
        }
    },
    ExcerptEmoji: function () {
        console.log(Group.el.excerpt.textContent)
        if (Group.el.excerpt.textContent) {
            Group.el.excerpt.innerHTML = Group.BasicEmojis.parseEmojis(
                Group.el.excerpt.textContent
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
            return `<img class="emoji" alt="emoji" src="~/images/emojis/${imageName}">`
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
                Group.ExcerptEmoji()
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
                Group.ExcerptEmoji()
            }
        } else {
            Group.newShowNotification(RoomId, message)
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
        Group.ExcerptEmoji()
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
                            const url = `${window.location.origin
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
                    ? Group.blockUser(el, 'block')
                    : Group.blockUser(el, 'report')
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

Group.init()

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