using System.Collections;
using System.Windows.Forms.Design;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Designer for DonkeyScroller
    /// </summary>
    public class DonkeyScrollerDesigner : ControlDesigner
    {
        /// <summary>
        /// Prefilters properties. Here we remove those we do not want to expose.
        /// </summary>
        /// <param name="properties">The properties of <see cref="DonkeyScroller"/></param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            properties.Remove("RightToLeft");
            properties.Remove("BackgroundImageLayout");
            properties.Remove("Cursor");
            properties.Remove("UseWaitCursor");
            properties.Remove("ImeMode");
            properties.Remove("TabIndex");
            properties.Remove("TabStop");
            properties.Remove("CausesValidation");
            base.PreFilterProperties(properties);
        }
        /// <summary>
        /// Prefilter events. Here we remove those we do not want to expose.
        /// </summary>
        /// <param name="events">The events of the <see cref="DonkeyScroller"/></param>
        protected override void PreFilterEvents(IDictionary events)
        {
            events.Remove(" Paint");
            events.Remove("ChangeUICues");
            events.Remove("ControlAdded");
            events.Remove("ControlRemoved");
            events.Remove("HelpRequested");
            events.Remove("ImeModeChanged");
            events.Remove("StyleChanged");
            events.Remove("SystemColorsChanged");
            events.Remove("Enter");
            events.Remove("Leave");
            events.Remove("Validated");
            events.Remove("Validating");
            events.Remove("KeyDown");
            events.Remove("KeyPress");
            events.Remove("KeyUp");
            events.Remove("PreviewKeyDown");
            events.Remove("Layout");
            events.Remove("MarginChanged");
            events.Remove("Move");
            events.Remove("PaddingChanged");
            events.Remove("Resize");
            events.Remove("BackgroundImageChanged");
            events.Remove("BackgroundImageLayoutChanged");
            events.Remove("BindingContextChanged");
            events.Remove("CausesValidationChanged");
            events.Remove("CursorChanged");
            events.Remove("RegionChanged");
            events.Remove("RightToLeftChanged");
            events.Remove("TabIndexChanged");
            events.Remove("TabStopChanged");
            events.Remove("TextChanged");
            base.PreFilterEvents(events);
        }
    }
}