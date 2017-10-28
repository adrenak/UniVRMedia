# Uni360Video
Stream 360 videos in Unity using its VideoPlayer component entirely through code and not editor setup.  
  
# Purpose
Unity introduced the much anticipated video player component in version 5.6  
[This official Unity blog](https://blogs.unity3d.com/2017/07/27/how-to-integrate-360-video-with-unity/) illustrates how to use the new video player to play 360 videos, however that is a skybox + RenderTexture approach, which may be expensive on mobiles, I dunot know. 

Another way is to have a camera in the centre of a sphere that has normals pointed inside and the Video Player component, but this requires manual setup.  
  
Uni360Video automates the manual workflow. It essentially follows these steps:
- Creates a primitive sphere at a position and inverts its normals
- Places a camera at the centre of the sphere
- Uses the Unlit Texture shader on the sphere surface and changes the color to your choice
- Gives you full control over playback by allowing access to the Video Player component in use
- Automatically disables all cameras when the 360 video starts and resets them back when the video is done
- Set the time scale to 0 and then back to its original value when the video is done

# How to use
- Check out `Uni360Video/Demo/DemoScene` for an example
- Use the `Uni360Video.Options` class to set initialization values for the 360 player. The class is also serializable in the editor.
- Use `Uni360Video.videoPlayer` to access the video player component in use

# Soon
- Better playback controls for video player
- Set a color gradient as the default color of the video surface instead of a solid color

# Notes
- Only supported on Unity 5.6+ This repository was made on 5.6.1, so to run on 5.6.0 you may have to delete the Library and ProjectSettings folder
- Currently only supports 360 panoramic videos
- Sample video used : [Visit the Philippines](https://www.youtube.com/watch?v=vQt2NRT5yP4)