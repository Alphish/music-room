using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Alphicsh.MusicRoom.View
{
    // courtesy of Even Lang, author of "Evan's Code Clunkers" blog
    // the blog itself can be found at https://evanl.wordpress.com/
    // the post this class is based on can be found here: https://evanl.wordpress.com/2009/12/06/efficient-optimal-per-frame-eventing-in-wpf/
    // I made a few changes to match my coding style (apparently it's important to me for some reason...?), but the idea and code is mostly by Evan

    /// <summary>
    /// Provides access to a static FrameUpdating event, launched every time the layout has been actually drawn on screen.
    /// </summary>
    public static class FrameEventProvider
    {
        // as soon as the event manager is used
        // the rendering event is added
        static FrameEventProvider()
            => CompositionTarget.Rendering += OnRendering;

        /// <summary>
        /// Occurs every displayed frame.
        /// </summary>
        public static event EventHandler<RenderingEventArgs> FrameUpdating;

        // represents the recent target time that occurred just before.
        private static TimeSpan RecentTargetTime = TimeSpan.Zero;

        // checks if the CompositionTarget.Rendering was launched during a new frame
        // and in such case, launches the frame updating event
        private static void OnRendering(object sender, EventArgs genericArgs)
        {
            var renderingArgs = genericArgs as RenderingEventArgs;
            if (renderingArgs.RenderingTime == RecentTargetTime)
                return;

            RecentTargetTime = renderingArgs.RenderingTime;
            FrameUpdating?.Invoke(sender, renderingArgs);
        }
    }
}
