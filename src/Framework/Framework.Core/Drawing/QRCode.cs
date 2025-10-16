
namespace Framework.Core.Drawing
{
    #region usings

    using ZXing;
    using ZXing.Common;
    using ZXing.QrCode;

    #endregion

    /// <summary>
    ///     The QR code util.
    /// </summary>
    public class QRCode
    {
        /// <summary>
        /// The generate simple string qr code.
        /// </summary>
        /// <param name="stringToEncode">
        /// The string to encode.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] GenerateSimpleStringQrCode(string stringToEncode, int height, int width)
        {
            // instantiate a writer object
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Height = height,
                    Width = width,
                    Margin = 5
                }
            };

            var image = barcodeWriter.Write(stringToEncode);

            return image.Pixels;
        }


        /// <summary>
        /// The decode qr code.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DecodeQrCode(byte[] image, int width, int height)
        {
            var source = new RGBLuminanceSource(image, width, height);
            var bitmap = new BinaryBitmap(new HybridBinarizer(source));
            var result = new MultiFormatReader().decode(bitmap);
            return result?.Text;
        }
    }
}