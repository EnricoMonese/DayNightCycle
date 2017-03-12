# Day-Night Cycle

 <img src="/gradient.gif" height="226" width="444">

This is my forked version of Enrico Monese's DayNightCycle. The skybox version is mostly identical to Enrico's. I added a version to use a gradient backdrop instead of the skybox procedural shader. The procedural skybox gives you very little control over the horizon line and adds a lot of complexity in terms of atmosphere and color. In my case, the sky never looks the color I actually want it to be. And, when doing realtime changes, I believe changing the skybox might have more performance overhead compared to using a gradient for global illumination.

The gradient versions give you more control over how the sky looks, good when you are making a 2D game or want more of a backdrop and less of a real atmosphere. The gradient you supply is used to build a new mesh based on where you've put the keys in your gradient. The resulting mesh is then matched to those keys, and vertex colors are assigned to the mesh for use by a simple vertex shader. Should be performant, even on mobile.  To render the backdrop, a new camera is made that matches your current camera, layers are set up and the new camera only renders the background image.

Video below shows it in action:

[![Alt text](https://img.youtube.com/vi/2SMf9vSy2IQ/0.jpg)](https://www.youtube.com/watch?v=2SMf9vSy2IQ)

For further instructions, visit the version on Enrico's git page.

## License


Copyright (C) 2016 Enrico Monese

MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


