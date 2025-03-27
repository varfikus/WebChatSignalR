document.addEventListener("DOMContentLoaded", function () {
    let mediaRecorder;
    let audioChunks = [];

    const recordButton = document.getElementById("recordButton");
    const sendButton = document.getElementById("sendButton");
    const audioPreview = document.getElementById("audioPreview");


    if (recordButton) {
        recordButton.addEventListener("click", async function () {
            if (!mediaRecorder || mediaRecorder.state === "inactive") {
                startRecording();
            } else {
                stopRecording();
            }
        });
    }

    async function startRecording() {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            mediaRecorder = new MediaRecorder(stream);
            audioChunks = [];

            mediaRecorder.ondataavailable = event => {
                if (event.data.size > 0) {
                    audioChunks.push(event.data);
                }
            };

            mediaRecorder.onstop = async () => {
                const audioBlob = new Blob(audioChunks, { type: "audio/webm" });
                audioPreview.src = URL.createObjectURL(audioBlob);
                audioPreview.classList.remove("d-none");

                await uploadAudio(audioBlob);
            };

            mediaRecorder.start();
            recordButton.textContent = "⏹️";
        } catch (error) {
            console.error("Ошибка доступа к микрофону", error);
        }
    }

    function stopRecording() {
        if (mediaRecorder && mediaRecorder.state !== "inactive") {
            mediaRecorder.stop();
            recordButton.textContent = "🎤";
        }
    }

    async function uploadAudio(blob) {
        const formData = new FormData();
        formData.append("audio", blob, "voice-message.webm");
        formData.append("RoomId", document.getElementById("conversationInput").value);
        formData.append("UserId", document.getElementById("userInput").value);

        try {
            const response = await fetch("/Chat/UploadVoiceMessage", {
                method: "POST",
                body: formData
            });

            if (response.ok) {
                console.log("Аудиофайл отправлен успешно");
            } else {
                console.error("Ошибка при отправке аудио");
            }
        } catch (error) {
            console.error("Ошибка сети", error);
        }
    }
});