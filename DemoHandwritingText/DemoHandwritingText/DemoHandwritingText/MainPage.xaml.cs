using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace DemoHandwritingText
{
	public partial class MainPage : ContentPage
	{

	    private readonly IVisionServiceClient _visionServiceClient;
	    private string url = "";

        public MainPage()
		{
			InitializeComponent();

            _visionServiceClient = new VisionServiceClient("KEY", "URL");

		}

	    private async void TakePicture(object sender, EventArgs e)
	    {
	        await CrossMedia.Current.Initialize();

	        if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
	        {
	            await DisplayAlert("Ops", "Nenhuma câmera detectada.", "OK");

	            return;
	        }

	        var file = await CrossMedia.Current.TakePhotoAsync(
	            new StoreCameraMediaOptions
	            {
	                SaveToAlbum = false,
	                Directory = "Demo",
	                PhotoSize = PhotoSize.Medium
	            });


	        if (file == null)
	            return;

	        await CreateHandwriting(file.AlbumPath);


            MinhaImagem.Source = ImageSource.FromStream(() =>
	        {
	            var stream = file.GetStream();
	            file.Dispose();


	            return stream;

	        });

	        await GetHandwriting(url);

	    }


	    public async Task CreateHandwriting(string image)
	    {
	        // Call the Vision API.
	        try
	        {
	            using (Stream imageFileStream = File.OpenRead(image))
	            {
	                var result = await _visionServiceClient.CreateHandwritingRecognitionOperationAsync(imageFileStream);

	                url = result.Url;

	            }
	        }
	        // Catch and display Vision API errors.
	        catch (ClientException c)
	        {
	            await DisplayAlert("Error", c.Message, "ok");
	        }
	        // Catch and display all other errors.
	        catch (Exception e)
	        {
	            await DisplayAlert("Error", e.Message, "ok");
	        }
	    }


	    public async Task GetHandwriting(string text)
	    {
	        HandwritingRecognitionOperation operation = new HandwritingRecognitionOperation();
	        operation.Url = text;
	        Resultado.Text = "";

	        // Call the Vision API.
	        try
	        {
	            var result = await _visionServiceClient.GetHandwritingRecognitionOperationResultAsync(operation);


	            foreach (var line in result.RecognitionResult.Lines)
	            {
	                foreach (var word in line.Words)
	                {
	                    Resultado.Text += word.Text + " ";
	                }
	            }
	        }
	        // Catch and display Vision API errors.
	        catch (ClientException c)
	        {
	            await DisplayAlert("Error", c.Message, "ok");
	        }
	        // Catch and display all other errors.
	        catch (Exception e)
	        {
	            await DisplayAlert("Error", e.Message, "ok");
	        }
	    }

    }
}
