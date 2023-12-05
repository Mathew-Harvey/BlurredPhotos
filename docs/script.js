Dropzone.options.uploadForm = {
    url: 'http://localhost:5035/image/upload',
    autoProcessQueue: false,
    maxFiles: 1,
    acceptedFiles: 'image/*',
    init: function () {
        var myDropzone = this;

        this.on("addedfile", function (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById('originalImage').src = e.target.result;
                document.getElementById('originalImage').style.display = 'block';
            };
            reader.readAsDataURL(file);
        });
        document.getElementById('submitImages').addEventListener('click', function () {
            if (myDropzone.files.length === 0) {
                alert('Please select a file to upload.');
                return;
            }
            var file = myDropzone.files[0];
            var formData = new FormData();
            formData.append('file', file);


            fetch(myDropzone.options.url, {
                method: 'POST',
                body: formData,
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok: ' + response.statusText);
                }
                return response.blob();  
            })
            .then(blob => {
                const imageUrl = URL.createObjectURL(blob); 
                document.getElementById('processedImage').src = imageUrl;  
                document.getElementById('processedImage').style.display = 'block';
            })
            .catch(error => {
                console.error('There has been a problem with your fetch operation:', error);
            });
        });
    }
};
