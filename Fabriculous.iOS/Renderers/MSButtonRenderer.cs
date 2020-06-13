using Fabriculous;
using Fabriculous.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using FButton = Microsoft.OfficeUIFabric.MSButton;

[assembly: ExportRenderer (typeof(MSButton), typeof(MSButtonRenderer))]
namespace Fabriculous.iOS.Renderers
{
    public class MSButtonRenderer : ViewRenderer<MSButton, FButton>
    {
        protected override void OnElementChanged (ElementChangedEventArgs<MSButton> e)
        {
            base.OnElementChanged (e);

            if (e.OldElement != null) {
                // Unsubscribe
            }
            if (e.NewElement != null) {
                if (Control == null) {
                    SetNativeControl (CreateNativeControl());
                    Control.SetTitle("Renderer", UIControlState.Normal);
                    Control.SetTitleColor(UIColor.White, UIControlState.Normal);
                    Control.BackgroundColor = UIColor.Orange;
                    Control.SetTitleColor(UIColor.Blue, UIControlState.Selected);
                }
                // Subscribe
            }
        }
        
        protected override FButton CreateNativeControl() => new FButton(Microsoft.OfficeUIFabric.MSButtonStyle.PrimaryFilled);

    }
}