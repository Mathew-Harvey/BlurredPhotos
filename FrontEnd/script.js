// Assuming Dropzone is included in your HTML or other script files
// and you have an HTML element with id 'submitImages' for the upload button

// Configure Dropzone for file selection
Dropzone.options.uploadForm = {
    url: 'http://localhost:5035/image/upload',
    autoProcessQueue: false,
    maxFiles: 1,
    acceptedFiles: 'image/*',
    init: function () {
        var myDropzone = this;

        // Handle the image preview on "addedfile" event
        this.on("addedfile", function (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById('originalImage').src = e.target.result;
                document.getElementById('originalImage').style.display = 'block';
            };
            reader.readAsDataURL(file);
        });

        // Handle the upload process when the button is clicked
        document.getElementById('submitImages').addEventListener('click', function () {
            if (myDropzone.files.length === 0) {
                alert('Please select a file to upload.');
                return;
            }
            var file = myDropzone.files[0];
            var formData = new FormData();
            formData.append('file', file);

            // Perform the upload using the Fetch API
            fetch(myDropzone.options.url, {
                method: 'POST',
                body: formData,
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok: ' + response.statusText);
                }
                return response.blob();  // This transforms the binary data into a blob object
            })
            .then(blob => {
                const imageUrl = URL.createObjectURL(blob);  // This creates a URL for the blob object
                document.getElementById('processedImage').src = imageUrl;  // This sets the blob URL as the image source
                document.getElementById('processedImage').style.display = 'block';
            })
            .catch(error => {
                console.error('There has been a problem with your fetch operation:', error);
            });
        });
    }
};
