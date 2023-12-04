// Configure Dropzone
Dropzone.options.uploadForm = {
    url: 'http://localhost:5035/image/upload', // Replace with your API endpoint
    autoProcessQueue: false,
    maxFiles: 1,
    acceptedFiles: 'image/*',
    init: function() {
        var myDropzone = this;

        // Handle the image preview on "addedfile" event
        this.on("addedfile", function(file) {
            var reader = new FileReader();
            reader.onload = function(e) {
                document.getElementById('originalImage').src = e.target.result;
                document.getElementById('originalImage').style.display = 'block';
            };
            reader.readAsDataURL(file);
        });

        // Process the queue when the button is clicked
        document.getElementById('submitImages').addEventListener('click', function() {
            myDropzone.processQueue(); // Process the images when button is clicked
        });

        this.on("success", function(file, response) {
            // Assuming the response contains the URL of the processed image
            document.getElementById('processedImage').src = response.processedImageUrl;
            document.getElementById('processedImage').style.display = 'block';
        });
    }
};
interact('.image-container').resizable({
    edges: { left: false, right: true, bottom: true, top: false },
    modifiers: [
        interact.modifiers.restrictSize({
            min: { width: 100, height: 100 }
        })
    ],
    inertia: true
}).on('resizemove', function (event) {
    var target = event.target;
    target.style.width = event.rect.width + 'px';
    target.style.height = event.rect.height + 'px';
});

