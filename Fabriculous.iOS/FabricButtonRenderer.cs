using System;
using System.ComponentModel;
using CoreGraphics;
using Fabriculous.iOS;
using MaterialComponents;
using Microsoft.OfficeUIFabric;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Button = Xamarin.Forms.Button;
using FButton = Microsoft.OfficeUIFabric.MSButton;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(FabricButtonRenderer), new[] { typeof(Fabriculous.FabricVisual) })]
namespace Fabriculous.iOS
{
    public class FabricButtonRenderer : ViewRenderer<Button, FButton>, IImageVisualElementRenderer, IButtonLayoutRenderer
    {
        bool _isDisposed;
        
        UIColor _buttonTextColorDefaultDisabled;
        UIColor _buttonTextColorDefaultHighlighted;
        UIColor _buttonTextColorDefaultNormal;

//        ButtonScheme _defaultButtonScheme;
//        ButtonScheme _buttonScheme;
        ButtonLayoutManager _buttonLayoutManager;

        public FabricButtonRenderer()
        {
            _buttonLayoutManager = new ButtonLayoutManager(this,
                preserveInitialPadding: true,
                spacingAdjustsPadding: false,
                borderAdjustsPadding: false,
                collapseHorizontalPadding: true);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (Control != null)
            {
                Control.TouchUpInside -= OnButtonTouchUpInside;
                Control.TouchDown -= OnButtonTouchDown;
                _buttonLayoutManager?.Dispose();
                _buttonLayoutManager = null;
            }

            _isDisposed = true;

            base.Dispose(disposing);
        }
        
        public override CGSize SizeThatFits(CGSize size)
        {
            var measured = base.SizeThatFits(size);
            return _buttonLayoutManager?.SizeThatFits(size, measured) ?? measured;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
//            _buttonScheme?.Dispose();
//            _buttonScheme = CreateButtonScheme();
            
            base.OnElementChanged(e);
            //Element.Text = "I am a Custom Visual Renderer";

            if (e.NewElement != null)
            {
                if (Control == null)
                {
//                    _defaultButtonScheme = CreateButtonScheme();

                    SetNativeControl(CreateNativeControl());
                    
                    SetControlPropertiesFromProxy();

                    _buttonTextColorDefaultNormal = Control.TitleColor(UIControlState.Normal);
                    _buttonTextColorDefaultHighlighted = Control.TitleColor(UIControlState.Highlighted);
                    _buttonTextColorDefaultDisabled = Control.TitleColor(UIControlState.Disabled);

                    Control.TouchUpInside += OnButtonTouchUpInside;
                    Control.TouchDown += OnButtonTouchDown;
                    
                }

                UpdateFont();
                UpdateCornerRadius();
                UpdateBorder();
                UpdateTextColor();
                _buttonLayoutManager?.Update();
                ApplyTheme();
            }
        }
        
        protected virtual ButtonScheme CreateButtonScheme()
        {
            return new ButtonScheme
            {
//                ColorScheme = MaterialColors.Light.CreateColorScheme(),
                ShapeScheme = new ShapeScheme(),
                TypographyScheme = new TypographyScheme(),
            };
        }

        protected virtual void ApplyTheme()
        {
//            ContainedButtonThemer.ApplyScheme(_buttonScheme, Control);
//            Control.BackgroundColor = _buttonScheme.ColorScheme.BackgroundColor;
            
//
//            // Colors have to be re-applied to Character spacing
            _buttonLayoutManager?.Update();
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
            }

            if (updatedTheme)
                ApplyTheme();
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

//        protected override void SetBackgroundColor(Color color)
//        {
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
//                
//                if (Control != null)
//                    ApplyTheme();
//            }
//        }

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
//                if (Element.Font == (Font) Button.FontProperty.DefaultValue)
//                    typographyScheme.Button = _defaultButtonScheme.TypographyScheme.Button;
//                else
//                    typographyScheme.Button = Element.Font.ToUIFont();
//            }
        }

        void UpdateTextColor()
        {
            if (Element.TextColor == Color.Default)
            {
                Control.SetTitleColor(_buttonTextColorDefaultNormal, UIControlState.Normal);
                Control.SetTitleColor(_buttonTextColorDefaultHighlighted, UIControlState.Highlighted);
                Control.SetTitleColor(_buttonTextColorDefaultDisabled, UIControlState.Disabled);
            }
            else
            {
                var color = Element.TextColor.ToUIColor();

                Control.SetTitleColor(color, UIControlState.Normal);
                Control.SetTitleColor(color, UIControlState.Highlighted);
                Control.SetTitleColor(color, UIControlState.Disabled);

                Control.TintColor = color;
            }
        }
        
        void SetControlPropertiesFromProxy()
        {
            foreach (UIControlState uiControlState in s_controlStates)
            {
                Control.SetTitleColor(UIButton.Appearance.TitleColor(uiControlState), uiControlState); // if new values are null, old values are preserved.
                Control.SetTitleShadowColor(UIButton.Appearance.TitleShadowColor(uiControlState), uiControlState);
                Control.SetBackgroundImage(UIButton.Appearance.BackgroundImageForState(uiControlState), uiControlState);
            }
        }
        
        static readonly UIControlState[] s_controlStates = { UIControlState.Normal, UIControlState.Highlighted, UIControlState.Disabled };


        // IImageVisualElementRenderer
        bool IImageVisualElementRenderer.IsDisposed => _isDisposed;
        void IImageVisualElementRenderer.SetImage(UIImage image) => _buttonLayoutManager.SetImage(image);
        UIImageView IImageVisualElementRenderer.GetImage() => Control?.ImageView;

        // IButtonLayoutRenderer
        UIButton IButtonLayoutRenderer.Control => Control;
        IImageVisualElementRenderer IButtonLayoutRenderer.ImageVisualElementRenderer => this;
        nfloat IButtonLayoutRenderer.MinimumHeight => -1;
    }
}