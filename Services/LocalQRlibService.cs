using QRCodeEncoderLibrary;
using System.Drawing;

namespace BlackBoxCheckApi.Services
{
    public class LocalQRlibService : IQRService
    {
        private QREncoder _qrcoder;
        public LocalQRlibService()
        {
            _qrcoder = new QREncoder();
            _qrcoder.ErrorCorrection = ErrorCorrection.Q;
        }
        public string GetBase64PngQRCode(string sourceString)
        {
            bool[,] QRMatrix = _qrcoder.Encode(sourceString);
            QRSavePngImage BitmapImage = new(QRMatrix);
            BitmapImage.ModuleSize = 5;
            var QRmap = Convert.ToBase64String(BitmapImage.QRCodeToPngFormat());
            return QRmap;
        }
    }
}
