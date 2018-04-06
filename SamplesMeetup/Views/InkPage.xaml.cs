using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.DirectX;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SamplesMeetup.Views
{
    public sealed partial class InkPage : Page
    {
        #region [ Fields ]
        private InkRecognizerContainer inkRecognizerContainer;
        #endregion [ Fields ]


        #region [ Constructors ]
        public InkPage()
        {
            this.InitializeComponent();

            // Set supported inking device types.
            if (this.inkCanvas?.InkPresenter != null)
                this.inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;

            #region Recognizer
            // Active recognizer
            // Show the available recognizers
            this.inkRecognizerContainer = new InkRecognizerContainer();

            // Show the available recognizers
            IReadOnlyList<InkRecognizer> recoView = this.inkRecognizerContainer.GetRecognizers();

            if (recoView?.Count > 0)
                this.inkRecognizerContainer.SetDefaultRecognizer(recoView[0]);
            #endregion Recognizer
        }
        #endregion [ Constructors ]


        #region [ Events ]
        #region [ Events - Controls ]
        /// <summary>
        /// Recognize text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonRecognizer_Click(object sender, RoutedEventArgs e)
        {
            this.textBlockResult.Text = String.Empty;

            IReadOnlyList<InkStroke> currentStrokes = this.inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            if (currentStrokes.Count > 0)
            {
                this.buttonRecognizer.IsEnabled = false;

                //Regonize
                IReadOnlyList<InkRecognitionResult> recognitionResults = await this.inkRecognizerContainer?.RecognizeAsync(this.inkCanvas?.InkPresenter?.StrokeContainer, InkRecognitionTarget.All);

                if (recognitionResults?.Count > 0)
                {
                    foreach (InkRecognitionResult item in recognitionResults)
                    {
                        if (item != null)
                            this.textBlockResult.Text += $"{item.GetTextCandidates()[0]} ";
                    }
                }

                this.buttonRecognizer.IsEnabled = true;
            }
        }


        /// <summary>
        /// Open Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonOpenImage_ClickAsync(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile fileImage = await picker.PickSingleFileAsync();

            if (fileImage != null)
            {
                IRandomAccessStream stream = await fileImage.OpenAsync(FileAccessMode.Read);

                // read bytes
                byte[] bytes = new byte[stream.Size];
                await stream.AsStream().ReadAsync(bytes, 0, bytes.Length);

                stream.AsStream().Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);
                this.image.Source = bitmapImage;
            }
        }


        /// <summary>
        /// Save image with Ink
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonSaveImage_ClickAsync(object sender, RoutedEventArgs e)
        {
            // The original bitmap from the screen, missing the ink.
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap();
            await renderBitmap.RenderAsync(this.gridInk);

            Size bitmapSizeAt96Dpi = new Size(renderBitmap.PixelWidth, renderBitmap.PixelHeight);

            IBuffer renderBitmapPixels = await renderBitmap.GetPixelsAsync();

            CanvasDevice win2DDevice = CanvasDevice.GetSharedDevice();

            DisplayInformation displayInfo = DisplayInformation.GetForCurrentView();

            using (CanvasRenderTarget win2DTarget = new CanvasRenderTarget(win2DDevice, (float)this.gridInk.ActualWidth, (float)this.gridInk.ActualHeight, 96.0f))
            {
                using (CanvasDrawingSession win2dSession = win2DTarget.CreateDrawingSession())
                {
                    using (CanvasBitmap win2dRenderedBitmap = CanvasBitmap.CreateFromBytes(win2DDevice, renderBitmapPixels, (int)bitmapSizeAt96Dpi.Width, (int)bitmapSizeAt96Dpi.Height, DirectXPixelFormat.B8G8R8A8UIntNormalized, 96.0f))
                    {
                        win2dSession.DrawImage(win2dRenderedBitmap, new Rect(0, 0, win2DTarget.SizeInPixels.Width, win2DTarget.SizeInPixels.Height), new Rect(0, 0, bitmapSizeAt96Dpi.Width, bitmapSizeAt96Dpi.Height));
                    }
                    win2dSession.Units = CanvasUnits.Pixels;
                    win2dSession.DrawInk(this.inkCanvas.InkPresenter.StrokeContainer.GetStrokes());
                }

                //Get file and save
                FileSavePicker picker = new FileSavePicker();
                picker.FileTypeChoices.Add("JPEG Image", new string[] { ".jpg" });
                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                        encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                             BitmapAlphaMode.Ignore,
                                             (uint)this.gridInk.ActualWidth, (uint)this.gridInk.ActualHeight,
                                             96, 96, win2DTarget.GetPixelBytes());

                        await encoder.FlushAsync();
                    }
                }
            }
        }
        #endregion [ Events - Controls ]
        #endregion [ Events ]
    }
}
