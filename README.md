# UniVRMedia
Stream 360 videos in Unity using its VideoPlayer component entirely through code and not editor setup.  
  
# Purpose
Unity introduced the much anticipated video player component in version 5.6  
[This official Unity blog](https://blogs.unity3d.com/2017/07/27/how-to-integrate-360-video-with-unity/) illustrates how to use the new video player to play 360 videos, however that is a skybox + RenderTexture approach, which may be expensive on mobiles, (I don't know). 

Another way is to have a camera in the centre of a sphere that has normals pointed inside and the Video Player component, but this requires manual setup.  
  
UniVRMedia automates the manual workflow. It essentially follows these steps:
- Creates a primitive sphere at a position and inverts its normals
- Places a camera at the centre of the sphere
- Uses the Unlit Texture shader on the sphere surface and changes the color to your choice
- Gives you full control over playback by allowing access to the Video Player component in use
- Automatically disables all cameras when the 360 video starts and resets them back when the video is done
- Set the time scale to 0 and then back to its original value when the video is done

# How to use
- Check out `UniVRMedia/Demo/DemoScene` for an example
- Use the `UniVRMedia.Options` class to set initialization values for the 360 video player. The class is also serializable in the editor.
- Use `UniVRMedia.Video` to access the video player component in use

# Notes
- Currently only supports 360 panoramic videos
- Sample video used : [Visit the Philippines](https://www.youtube.com/watch?v=vQt2NRT5yP4)

# Contact
[Twitter](https://www.twitter.com/adrenak)  
[Github](https://www.github.com/adrenak)