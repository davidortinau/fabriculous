using System;
using System.ComponentModel;
using CoreGraphics;
using Fabriculous.iOS;
using Microsoft.OfficeUIFabric;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using FButton = Microsoft.OfficeUIFabric.MSButton;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(FabricButtonRenderer), new[] { typeof(Fabriculous.FabricVisual) })]
namespace Fabriculous.iOS
{
    public class FabricButtonRenderer : ViewRenderer<Button, FButton>
    {
        //private ButtonScheme _defaultButtonScheme;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            //Element.Text = "I am a Custom Visual Renderer";

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    //_defaultButtonScheme = CreateButtonScheme();

                    SetNativeControl(CreateNativeControl());
                    
                    
                    Control.SetTitle(Element.Text,UIKit.UIControlState.Normal);
//                    Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
                    Control.SetTitleColor(Element.TextColor.ToUIColor(), UIKit.UIControlState.Normal);
                    Control.TouchUpInside += OnButtonTouchUpInside;
                    Control.TouchDown += OnButtonTouchDown;                    
                }

                UpdateFont();
                UpdateCornerRadius();
                UpdateBorder();
                UpdateBackgroundColor();
                UpdateTextColor();
                ApplyTheme();
            }
        }

        protected virtual void ApplyTheme()
        {
//            ContainedButtonThemer.ApplyScheme(_buttonScheme, Control);
//
//            // Colors have to be re-applied to Character spacing
//            _buttonLayoutManager?.UpdateText();
        }

        protected override FButton CreateNativeControl() => new FButton(Microsoft.OfficeUIFabric.MSButtonStyle.PrimaryFilled);

        void OnButtonTouchUpInside(object sender, EventArgs eventArgs)
        {
            Element?.SendReleased();
            Element?.SendClicked();
        }

        void OnButtonTouchDown(object sender, EventArgs eventArgs) => Element?.SendPressed();

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var updatedTheme = false;
            if (e.PropertyName == Button.TextColorProperty.PropertyName)
            {
                UpdateTextColor();
                updatedTheme = true;
            }
            else if (e.PropertyName == Button.FontProperty.PropertyName)
            {
                UpdateFont();
                updatedTheme = true;
            }
            else if (e.PropertyName == Button.BorderWidthProperty.PropertyName || e.PropertyName == Button.BorderColorProperty.PropertyName)
            {
                UpdateBorder();
            }
            else if (e.PropertyName == Button.CornerRadiusProperty.PropertyName)
            {
                UpdateCornerRadius();
                updatedTheme = true;
            }else if (e.PropertyName == Button.BackgroundColorProperty.PropertyName)
            {
                UpdateBackgroundColor();
            }

            //if (updatedTheme)
            //    ApplyTheme();
        }

        protected override void SetAccessibilityLabel()
        {
            // If we have not specified an AccessibilityLabel and the AccessibilityLabel is currently bound to the Title,
            // exit this method so we don't set the AccessibilityLabel value and break the binding.
            // This may pose a problem for users who want to explicitly set the AccessibilityLabel to null, but this
            // will prevent us from inadvertently breaking UI Tests that are using Query.Marked to get the dynamic Title
            // of the Button.

            var elemValue = (string)Element?.GetValue(AutomationProperties.NameProperty);
            if (string.IsNullOrWhiteSpace(elemValue) && Control?.AccessibilityLabel == Control?.Title(UIControlState.Normal))
                return;

            base.SetAccessibilityLabel();
        }

        protected override void SetBackgroundColor(Color color)
        {
//            if (_buttonScheme?.ColorScheme is SemanticColorScheme colorScheme)
//            {
//                if (color.IsDefault)
//                {
//                    colorScheme.PrimaryColor = _defaultButtonScheme.ColorScheme.PrimaryColor;
//                    colorScheme.OnSurfaceColor = _defaultButtonScheme.ColorScheme.OnSurfaceColor;
//                }
//                else
//                {
//                    UIColor uiColor = color.ToUIColor();
//
//                    colorScheme.PrimaryColor = uiColor;
//                    colorScheme.OnSurfaceColor = uiColor;
//                }
//            }
        }

        private CGColor _defaultBorderColor;

        private nfloat _defaultBorderWidth;
        
        void UpdateBorder()
        {
            // NOTE: borders are not a "supported" style of the contained
            // button, thus we don't use the themer here.
            Color borderColor = Element.BorderColor;

            if (_defaultBorderColor == null)
                _defaultBorderColor = Control.Layer.BorderColor;
//
            if (borderColor.IsDefault)
                Control.Layer.BorderColor = _defaultBorderColor;
            else
                Control.Layer.BorderColor = borderColor.ToCGColor();

            double borderWidth = Element.BorderWidth;

            if (_defaultBorderWidth == -1)
                _defaultBorderWidth = Control.Layer.BorderWidth;
            
            if (borderWidth == (double)Button.BorderWidthProperty.DefaultValue)
                Control.Layer.BorderWidth = _defaultBorderWidth;
            else
                Control.Layer.BorderWidth = (nfloat)borderWidth;
        }

        void UpdateCornerRadius()
        {
            int cornerRadius = Element.CornerRadius;

            if (cornerRadius == (int)Button.CornerRadiusProperty.DefaultValue)
            {
                Control.Layer.CornerRadius = 8.0f; // Constants on MSButton don't appear to be available
            }
            else
            {
                Control.Layer.CornerRadius = cornerRadius;
            }
        }

        void UpdateFont()
        {
//            if (_buttonScheme.TypographyScheme is TypographyScheme typographyScheme)
//            {
//                if (Element.Font == (Font)Button.FontProperty.DefaultValue)
//                    typographyScheme.Button = _defaultButtonScheme.TypographyScheme.Button;
//                else
//                    typographyScheme.Button = Element.ToUIFont();
//            }
        }

        void UpdateTextColor()
        {
                Color textColor = Element.TextColor;

                if (textColor.IsDefault)
                    Control.SetTitleColor(MSColors.ForegroundRegular, UIKit.UIControlState.Normal);
                else
                    Control.SetTitleColor(textColor.ToUIColor(), UIKit.UIControlState.Normal);

        }

        void UpdateBackgroundColor()
        {
            Color backgroundColor = Element.BackgroundColor;
            
            if(backgroundColor.IsDefault)
                Control.BackgroundColor = MSColors.Primary;
            else
                Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
        }

        //// IImageVisualElementRenderer
        //bool IImageVisualElementRenderer.IsDisposed => _isDisposed;
        //void IImageVisualElementRenderer.SetImage(UIImage image) => _buttonLayoutManager.SetImage(image);
        //UIImageView IImageVisualElementRenderer.GetImage() => Control?.ImageView;

        //// IButtonLayoutRenderer
        //UIButton IButtonLayoutRenderer.Control => Control;
        //IImageVisualElementRenderer IButtonLayoutRenderer.ImageVisualElementRenderer => this;
        //nfloat IButtonLayoutRenderer.MinimumHeight => _buttonScheme?.MinimumHeight ?? -1;
    }
}