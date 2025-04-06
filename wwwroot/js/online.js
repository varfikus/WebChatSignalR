'use strict';

var OnlineStatus = {
    connection: null,
    targetUserId: null,

    init: function () {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.connection.on("UserOnline", function (userId) {
            OnlineStatus.setUserOnline(userId);
        });

        this.connection.on("UserOffline", function (userId) {
            OnlineStatus.setUserOffline(userId);
        });

        this.connection.start()
            .then(() => {
                console.log("OnlineStatus connection started");
                OnlineStatus.registerOnline();

                // При старте подключение, проверяем статус собеседника (возможно, нужно вызвать API)
                const targetUserId = OnlineStatus.targetUserId;
                if (targetUserId) {
                    OnlineStatus.checkInitialStatus(targetUserId);
                }
            })
            .catch(err => console.error("SignalR connection error: ", err));
    },

    registerOnline: function () {
        const currentUserId = document.getElementById('userInput')?.value;
        if (currentUserId) {
            this.connection.invoke("RegisterOnline", currentUserId)
                .catch(err => console.error("RegisterOnline error: ", err));
        }
    },

    setUserOnline: function (userId) {
        // Обновление статуса в списке пользователей
        const userElement = document.querySelector(`.user-link[data-UserId="${userId}"]`);
        if (userElement) {
            userElement.classList.add("online");
            let badge = userElement.querySelector(".status-badge");
            if (!badge) {
                badge = document.createElement("span");
                badge.className = "status-badge online";
                userElement.querySelector(".profile-avatar").appendChild(badge);
            } else {
                badge.classList.remove("offline");
                badge.classList.add("online");
            }
        }

        // Обновление в окне чата
        if (OnlineStatus.targetUserId && OnlineStatus.targetUserId === userId) {
            const statusElem = document.getElementById("user-status");
            if (statusElem) {
                statusElem.textContent = "Online";
                statusElem.classList.remove("text-muted");
                statusElem.classList.add("text-success");
            }
        }
    },

    setUserOffline: function (userId) {
        const userElement = document.querySelector(`.user-link[data-UserId="${userId}"]`);
        if (userElement) {
            userElement.classList.remove("online");
            let badge = userElement.querySelector(".status-badge");
            if (!badge) {
                badge = document.createElement("span");
                badge.className = "status-badge offline";
                userElement.querySelector(".profile-avatar").appendChild(badge);
            } else {
                badge.classList.remove("online");
                badge.classList.add("offline");
            }
        }

        if (OnlineStatus.targetUserId && OnlineStatus.targetUserId === userId) {
            const statusElem = document.getElementById("user-status");
            if (statusElem) {
                statusElem.textContent = "Offline";
                statusElem.classList.remove("text-success");
                statusElem.classList.add("text-muted");
            }
        }
    },

    setTargetUserId: function (userId) {
        this.targetUserId = userId;
    },

    checkInitialStatus: function (targetUserId) {
        this.connection.invoke("CheckUserStatus", targetUserId)
            .then(isOnline => {
                if (isOnline) {
                    this.setUserOnline(targetUserId);
                } else {
                    this.setUserOffline(targetUserId);
                }
            })
            .catch(err => console.error("CheckUserStatus error: ", err));
    }
};

document.addEventListener("DOMContentLoaded", function () {
    OnlineStatus.init();

    const targetIdElem = document.getElementById("targetUserId");
    if (targetIdElem) {
        OnlineStatus.setTargetUserId(targetIdElem.value);
    }
});
