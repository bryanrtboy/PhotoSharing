
![screen capture](screencapture.jpg)

# Photo Gallery
An app to take a screenshot and save it to an in-game gallery on all platforms using Unity.  Surprisingly complicated to set this up!  The app I am designing this for has the following requirements:
* Save a screenshot to disc as if the user was using their camera
* Can be used in landscape or portrait mode
* Saves 4 images to disc, so closing/opening the app will retain the images
* Allow for deleting of images from within the app
* Once 4 images are saved, older ones are deleted so the app doesn't keep taking up space
* Thumbnails and large preview images must display correctly by resizing aspect ratio based on screen orientation
* If orientation changes while viewing the preview image, image updates to be correct again
* Some UI elements are hidden during screen capture, and other UI elements become part of the screen capture (border, logo)

# Sharing
This uses yasirkula's excellent plugin found here: https://github.com/yasirkula/UnityNativeGallery to allow the saved photos to be added to the devices Photo Album. That plug-in is installed via the manifest.json in this project.

Once a photo has been successfully shared to the device album, it is deleted from the in-game app gallery.
# Basic UI concept
The example scene controls the UI by linking and turning on/off things as needed to simulate a camera experience. So the buttons are hard-linked in the editor and the main script also references these buttons as thumbnails for the gallery. At startup, the contents of the user disc is read and those image names are then used to name the buttons. This is how the Delete and Featured image area keeps track of what image is being viewed and potentially deleted.
