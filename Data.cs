using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections.Generic;

/////////////////////////////////////////////////////////////////////////////////////////////
/// 【クラス名】Data
/// 【機  能】staticなデータを全て記述する
/////////////////////////////////////////////////////////////////////////////////////////////

enum EnumImageFormat
{
    Png, Bmp
};

enum EnumBrushType
{
    Solid, Image, Gradient
};

namespace BMPFontMaker
{
    static class Data
    {
        ///
        /// メンバ変数
        /// 

        // スムージング
        public static System.Drawing.Text.TextRenderingHint Smooth;

        // 背景色
        public static Color BgColor;

        // 罫線色
        public static Color LineColor;

        // イメージサイズ
        public static Size ImageSize;

        // バディング
        public static int PaddingTop, PaddingBottom, PaddingLeft, PaddingRight;

        // フォント
        public static Font DrawFont;

        // フォント色
        public static EnumBrushType DrawFontColorType;
        public static Color DrawFontColor;

        // ファイルフォーマット
        public static EnumImageFormat SaveImageFormat;

        // 縁取り
        public static bool Edge;
        public static bool EdgeAntialias;
        public static Color EdgeColor;
        public static EnumBrushType EdgeColorType;
        public static float EdgePenWidth;
        public static bool EdgeFirst;
        public static bool EdgeOnly;

        // ブラシ用ビットマップイメージ
        public static Bitmap DrawFontColorImage;
        public static Bitmap EdgeColorImage;

        // ブラシ用ビットマップイメージファイル名
        public static string DrawFontColorImageFilename;
        public static string EdgeColorImageFilename;

        // ブラシ用ビットマップイメージスケーリングフラグ
        public static bool DrawFontColorImageScaling;
        public static bool EdgeColorImageScaling;

        // グラデーションブラシ設定(フォント)
        public static int FontGradientWidth, FontGradientHeight, FontGradientAngle, FontGradientStartX, FontGradientStartY, FontGradientEndX, FontGradientEndY;
        public static PointSetting[] FontGradientData = new PointSetting[SetLinearGradientBrushForm.BrushDataMaxCount];
        public static WrapMode FontGradientWrapMode;

        // グラデーションブラシ設定(縁取り)
        public static int EdgeGradientWidth, EdgeGradientHeight, EdgeGradientAngle, EdgeGradientStartX, EdgeGradientStartY, EdgeGradientEndX, EdgeGradientEndY;
        public static PointSetting[] EdgeGradientData = new PointSetting[SetLinearGradientBrushForm.BrushDataMaxCount];
        public static WrapMode EdgeGradientWrapMode;

        // 文字指定
        public static bool HankakuKigou;
        public static bool HankakuNumber;
        public static bool HankakuAlphabet;
        public static bool HankakuKatakana;

        public static bool ZenkakuKigou;
        public static bool ZenkakuNumber;
        public static bool ZenkakuAlphabet;
        public static bool ZenkakuHiragana;
        public static bool ZenkakuKatakana;
        public static bool ZenkakuRussian;
        public static bool ZenkakuLine;
        public static bool ZenkakuOthers;

        public static bool KanjiExists;
        public static bool KanjiElementary1;
        public static bool KanjiElementary2;
        public static bool KanjiElementary3;
        public static bool KanjiElementary4;
        public static bool KanjiElementary5;
        public static bool KanjiElementary6;
        public static bool KanjiMiddle;
        public static bool KanjiName;
        public static bool KanjiOther;

        public static bool OthersExists;
        public static string OthersList;

        // XMLオプション
        public static bool XMLFixWidthEnable;
        public static int XMLFixWidth;
        public static bool XMLUReverse;
        public static bool XMLVReverse;
        public static bool XMLEx;

        // 生成したビットマップ一覧
        public static List<Bitmap> listBitmap;

        // フォント設定保持
        private static string TempFontName;
        private static bool TempFontBold;
        private static bool TempFontItalic;
        private static bool TempFontStrikeout;
        private static bool TempFontUnderline;
        private static float TempFontSize;

        // 設定ファイルパス
        public static string Filepath;

        // 設定保存
        public static void SaveFile()
        {
            SaveFile(Filepath);
        }
        public static void SaveFile(string SaveFilePath)
        {
            try
            {
                XMLTagWriterClass xml = new XMLTagWriterClass(SaveFilePath);

                xml.BeginElement("Settings");
                xml.WriteElement("Smooth", ((int)Smooth).ToString());
                xml.WriteElement("SaveImageFormat", ((int)SaveImageFormat).ToString());
                xml.WriteElement("BgColor", ColorToHTML(BgColor));
                xml.WriteElement("LineColor", ColorToHTML(LineColor));
                xml.WriteElement("ImageSize_Width", ImageSize.Width.ToString());
                xml.WriteElement("ImageSize_Height", ImageSize.Height.ToString());
                xml.WriteElement("PaddingTop", PaddingTop.ToString());
                xml.WriteElement("PaddingBottom", PaddingBottom.ToString());
                xml.WriteElement("PaddingLeft", PaddingLeft.ToString());
                xml.WriteElement("PaddingRight", PaddingRight.ToString());
                xml.WriteElement("Font_Name", DrawFont.Name);
                xml.WriteElement("Font_Bold", DrawFont.Bold.ToString());
                xml.WriteElement("Font_Italic", DrawFont.Italic.ToString());
                xml.WriteElement("Font_Strikeout", DrawFont.Strikeout.ToString());
                xml.WriteElement("Font_Underline", DrawFont.Underline.ToString());
                xml.WriteElement("Font_Size", DrawFont.Size.ToString());
                xml.WriteElement("DrawFontColor", ColorToHTML(DrawFontColor));
                xml.WriteElement("DrawFontColorType", ((int)DrawFontColorType).ToString());
                if (DrawFontColorType == EnumBrushType.Image)
                {
                    xml.WriteElement("DrawFontColorImageFilename", DrawFontColorImageFilename);
                    xml.WriteElement("DrawFontColorImageScaling", DrawFontColorImageScaling.ToString());
                }
                xml.WriteElement("Edge", Edge.ToString());
                xml.WriteElement("EdgeAntialias", EdgeAntialias.ToString());
                xml.WriteElement("EdgeColor", ColorToHTML(EdgeColor));
                xml.WriteElement("EdgeColorType", ((int)EdgeColorType).ToString());
                if (EdgeColorType == EnumBrushType.Image)
                {
                    xml.WriteElement("EdgeColorImageFilename", EdgeColorImageFilename);
                    xml.WriteElement("EdgeColorImageScaling", EdgeColorImageScaling.ToString());
                }
                xml.WriteElement("EdgePenWidth", String.Format("{0:F2}", EdgePenWidth));
                xml.WriteElement("EdgeFirst", EdgeFirst.ToString());
                xml.WriteElement("EdgeOnly", EdgeOnly.ToString());

                xml.WriteElement("FontGradientWidth", FontGradientWidth.ToString());
                xml.WriteElement("FontGradientHeight", FontGradientHeight.ToString());
                xml.WriteElement("FontGradientAngle", FontGradientAngle.ToString());
                xml.WriteElement("FontGradientStartX", FontGradientStartX.ToString());
                xml.WriteElement("FontGradientStartY", FontGradientStartY.ToString());
                xml.WriteElement("FontGradientEndX", FontGradientEndX.ToString());
                xml.WriteElement("FontGradientEndY", FontGradientEndY.ToString());
                xml.WriteElement("FontGradientWrapMode", ((int)FontGradientWrapMode).ToString());
                xml.WriteElement("FontGradientData1_Enable", FontGradientData[0].bEnable.ToString());
                xml.WriteElement("FontGradientData1_Color", ColorToHTML(FontGradientData[0].GradientColor));
                xml.WriteElement("FontGradientData1_Position", FontGradientData[0].nPosition.ToString());
                xml.WriteElement("FontGradientData2_Enable", FontGradientData[1].bEnable.ToString());
                xml.WriteElement("FontGradientData2_Color", ColorToHTML(FontGradientData[1].GradientColor));
                xml.WriteElement("FontGradientData2_Position", FontGradientData[1].nPosition.ToString());
                xml.WriteElement("FontGradientData3_Enable", FontGradientData[2].bEnable.ToString());
                xml.WriteElement("FontGradientData3_Color", ColorToHTML(FontGradientData[2].GradientColor));
                xml.WriteElement("FontGradientData3_Position", FontGradientData[2].nPosition.ToString());
                xml.WriteElement("FontGradientData4_Enable", FontGradientData[3].bEnable.ToString());
                xml.WriteElement("FontGradientData4_Color", ColorToHTML(FontGradientData[3].GradientColor));
                xml.WriteElement("FontGradientData4_Position", FontGradientData[3].nPosition.ToString());
                xml.WriteElement("FontGradientData5_Enable", FontGradientData[4].bEnable.ToString());
                xml.WriteElement("FontGradientData5_Color", ColorToHTML(FontGradientData[4].GradientColor));
                xml.WriteElement("FontGradientData5_Position", FontGradientData[4].nPosition.ToString());
                xml.WriteElement("FontGradientData6_Enable", FontGradientData[5].bEnable.ToString());
                xml.WriteElement("FontGradientData6_Color", ColorToHTML(FontGradientData[5].GradientColor));
                xml.WriteElement("FontGradientData6_Position", FontGradientData[5].nPosition.ToString());

                xml.WriteElement("EdgeGradientWidth", EdgeGradientWidth.ToString());
                xml.WriteElement("EdgeGradientHeight", EdgeGradientHeight.ToString());
                xml.WriteElement("EdgeGradientAngle", EdgeGradientAngle.ToString());
                xml.WriteElement("EdgeGradientStartX", EdgeGradientStartX.ToString());
                xml.WriteElement("EdgeGradientStartY", EdgeGradientStartY.ToString());
                xml.WriteElement("EdgeGradientEndX", EdgeGradientEndX.ToString());
                xml.WriteElement("EdgeGradientEndY", EdgeGradientEndY.ToString());
                xml.WriteElement("EdgeGradientWrapMode", ((int)EdgeGradientWrapMode).ToString());
                xml.WriteElement("EdgeGradientData1_Enable", EdgeGradientData[0].bEnable.ToString());
                xml.WriteElement("EdgeGradientData1_Color", ColorToHTML(EdgeGradientData[0].GradientColor));
                xml.WriteElement("EdgeGradientData1_Position", EdgeGradientData[0].nPosition.ToString());
                xml.WriteElement("EdgeGradientData2_Enable", EdgeGradientData[1].bEnable.ToString());
                xml.WriteElement("EdgeGradientData2_Color", ColorToHTML(EdgeGradientData[1].GradientColor));
                xml.WriteElement("EdgeGradientData2_Position", EdgeGradientData[1].nPosition.ToString());
                xml.WriteElement("EdgeGradientData3_Enable", EdgeGradientData[2].bEnable.ToString());
                xml.WriteElement("EdgeGradientData3_Color", ColorToHTML(EdgeGradientData[2].GradientColor));
                xml.WriteElement("EdgeGradientData3_Position", EdgeGradientData[2].nPosition.ToString());
                xml.WriteElement("EdgeGradientData4_Enable", EdgeGradientData[3].bEnable.ToString());
                xml.WriteElement("EdgeGradientData4_Color", ColorToHTML(EdgeGradientData[3].GradientColor));
                xml.WriteElement("EdgeGradientData4_Position", EdgeGradientData[3].nPosition.ToString());
                xml.WriteElement("EdgeGradientData5_Enable", EdgeGradientData[4].bEnable.ToString());
                xml.WriteElement("EdgeGradientData5_Color", ColorToHTML(EdgeGradientData[4].GradientColor));
                xml.WriteElement("EdgeGradientData5_Position", EdgeGradientData[4].nPosition.ToString());
                xml.WriteElement("EdgeGradientData6_Enable", EdgeGradientData[5].bEnable.ToString());
                xml.WriteElement("EdgeGradientData6_Color", ColorToHTML(EdgeGradientData[5].GradientColor));
                xml.WriteElement("EdgeGradientData6_Position", EdgeGradientData[5].nPosition.ToString());

                xml.WriteElement("XMLFixWidthEnable", XMLFixWidthEnable.ToString());
                xml.WriteElement("XMLFixWidth", XMLFixWidth.ToString());
                xml.WriteElement("XMLUReverse", XMLUReverse.ToString());
                xml.WriteElement("XMLVReverse", XMLVReverse.ToString());
                xml.WriteElement("XMLEx", XMLEx.ToString());

                xml.WriteElement("HankakuKigou", HankakuKigou.ToString());
                xml.WriteElement("HankakuNumber", HankakuNumber.ToString());
                xml.WriteElement("HankakuAlphabet", HankakuAlphabet.ToString());
                xml.WriteElement("HankakuKatakana", HankakuKatakana.ToString());
                xml.WriteElement("ZenkakuKigou", ZenkakuKigou.ToString());
                xml.WriteElement("ZenkakuNumber", ZenkakuNumber.ToString());
                xml.WriteElement("ZenkakuAlphabet", ZenkakuAlphabet.ToString());
                xml.WriteElement("ZenkakuHiragana", ZenkakuHiragana.ToString());
                xml.WriteElement("ZenkakuKatakana", ZenkakuKatakana.ToString());
                xml.WriteElement("ZenkakuRussian", ZenkakuRussian.ToString());
                xml.WriteElement("ZenkakuLine", ZenkakuLine.ToString());
                xml.WriteElement("ZenkakuOthers", ZenkakuOthers.ToString());
                xml.WriteElement("KanjiExists", KanjiExists.ToString());
                xml.WriteElement("KanjiElementary1", KanjiElementary1.ToString());
                xml.WriteElement("KanjiElementary2", KanjiElementary2.ToString());
                xml.WriteElement("KanjiElementary3", KanjiElementary3.ToString());
                xml.WriteElement("KanjiElementary4", KanjiElementary4.ToString());
                xml.WriteElement("KanjiElementary5", KanjiElementary5.ToString());
                xml.WriteElement("KanjiElementary6", KanjiElementary6.ToString());
                xml.WriteElement("KanjiMiddle", KanjiMiddle.ToString());
                xml.WriteElement("KanjiName", KanjiName.ToString());
                xml.WriteElement("KanjiOther", KanjiOther.ToString());
                xml.WriteElement("OthersExists", OthersExists.ToString());
                xml.WriteElement("OthersList", OthersList);
                xml.EndElement();
                xml.Close();
                xml.Dispose();

                // ファイルパスを保存
                Filepath = SaveFilePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("ファイルの出力に失敗しました({0}):{1}", SaveFilePath, ex.Message)
                    , "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

            }
        }

        // 設定オープン
        public static void LoadFile(string OpenFilePath)
        {
            string strElement = "", strValue = "";
            XMLTagReaderClass xml = new XMLTagReaderClass(OpenFilePath);

            while (xml.Read())
            {
                if (xml.NodeType == System.Xml.XmlNodeType.Element)
                {
                    // エレメント名を保持
                    strElement = xml.Value;
                    strValue = "";
                }
                else if (xml.NodeType == System.Xml.XmlNodeType.Text)
                {
                    // テキストを保持
                    strValue = xml.Value;
                }
                else if (xml.NodeType == System.Xml.XmlNodeType.EndElement)
                {
                    if (xml.Value == strElement)
                    {
                        // 取得済みのテキストを確定させる
                        LoadFile_SetValue(strElement, strValue);
                    }
                }
            }

            // フォントの設定反映
            FontStyle style;

            style = 0;
            if (TempFontBold)
            {
                style |= FontStyle.Bold;
            }
            if (TempFontItalic)
            {
                style |= FontStyle.Italic;
            }
            if (TempFontStrikeout)
            {
                style |= FontStyle.Strikeout;
            }
            if (TempFontUnderline)
            {
                style |= FontStyle.Underline;
            }
            if (style == 0)
            {
                style = FontStyle.Regular;
            }

            DrawFont = new Font(new FontFamily(TempFontName), TempFontSize, style);

            // ファイルパスを保存
            Filepath = OpenFilePath;
        }

        // XMLから取得した設定をメンバ変数に反映させる
        private static void LoadFile_SetValue(string strElement, string strValue)
        {
            int nIntValue;
            float fFloatValue;

            if (!Int32.TryParse(strValue, out nIntValue))
            {
                nIntValue = 0;
            }
            if (!float.TryParse(strValue, out fFloatValue))
            {
                fFloatValue = 0.0f;
            }

            if (strElement == "Smooth")
            {
                // int型を列挙型に変換
                Smooth = (System.Drawing.Text.TextRenderingHint)Enum.ToObject(typeof(System.Drawing.Text.TextRenderingHint), nIntValue);
            } else if (strElement == "SaveImageFormat")
            {
                // int型を列挙型に変換
                SaveImageFormat = (EnumImageFormat)Enum.ToObject(typeof(EnumImageFormat), nIntValue);
            }
            else if (strElement == "BgColor")
            {
                BgColor = HTMLToColor(strValue);
            }
            else if (strElement == "LineColor")
            {
                LineColor = HTMLToColor(strValue);
            }
            else if (strElement == "ImageSize_Width")
            {
                ImageSize.Width = nIntValue;
            }
            else if (strElement == "ImageSize_Height")
            {
                ImageSize.Height = nIntValue;
            }
            else if (strElement == "PaddingTop")
            {
                PaddingTop = nIntValue;
            }
            else if (strElement == "PaddingBottom")
            {
                PaddingBottom = nIntValue;
            }
            else if (strElement == "PaddingLeft")
            {
                PaddingLeft = nIntValue;
            }
            else if (strElement == "PaddingRight")
            {
                PaddingRight = nIntValue;
            }
            else if (strElement == "Font_Name")
            {
                TempFontName = strValue;
            }
            else if (strElement == "Font_Bold")
            {
                TempFontBold = Convert.ToBoolean(strValue);
            }
            else if (strElement == "Font_Italic")
            {
                TempFontItalic = Convert.ToBoolean(strValue);
            }
            else if (strElement == "Font_Strikeout")
            {
                TempFontStrikeout = Convert.ToBoolean(strValue);
            }
            else if (strElement == "Font_Underline")
            {
                TempFontUnderline = Convert.ToBoolean(strValue);
            }
            else if (strElement == "Font_Size")
            {
                TempFontSize = fFloatValue;
            }
            else if (strElement == "DrawFontColor")
            {
                DrawFontColor = HTMLToColor(strValue);
            }
            else if (strElement == "DrawFontColorType")
            {
                DrawFontColorType = (EnumBrushType)Enum.ToObject(typeof(EnumBrushType), nIntValue);
            }
            else if (strElement == "DrawFontColorImageFilename")
            {
                DrawFontColorImageFilename = strValue;
            }
            else if (strElement == "DrawFontColorImageScaling")
            {
                DrawFontColorImageScaling = Convert.ToBoolean(strValue);
            }
            else if (strElement == "Edge")
            {
                Edge = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeAntialias")
            {
                EdgeAntialias = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeColor")
            {
                EdgeColor = HTMLToColor(strValue);
            }
            else if (strElement == "EdgeColorType")
            {
                EdgeColorType = (EnumBrushType)Enum.ToObject(typeof(EnumBrushType), nIntValue);
            }
            else if (strElement == "EdgeColorImageFilename")
            {
                EdgeColorImageFilename = strValue;
            }
            else if (strElement == "EdgeColorImageScaling")
            {
                EdgeColorImageScaling = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgePenWidth")
            {
                EdgePenWidth = (float)Convert.ToDouble(strValue);
            }
            else if (strElement == "EdgeFirst")
            {
                EdgeFirst = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeOnly")
            {
                EdgeOnly = Convert.ToBoolean(strValue);
            }
            else if (strElement == "FontGradientWidth")
            {
                FontGradientWidth = nIntValue;
            }
            else if (strElement == "FontGradientHeight")
            {
                FontGradientHeight = nIntValue;
            }
            else if (strElement == "FontGradientAngle")
            {
                FontGradientAngle = nIntValue;
            }
            else if (strElement == "FontGradientStartX")
            {
                FontGradientStartX = nIntValue;
            }
            else if (strElement == "FontGradientStartY")
            {
                FontGradientStartY = nIntValue;
            }
            else if (strElement == "FontGradientEndX")
            {
                FontGradientEndX = nIntValue;
            }
            else if (strElement == "FontGradientEndY")
            {
                FontGradientEndY = nIntValue;
            }
            else if (strElement == "FontGradientWrapMode")
            {
                FontGradientWrapMode = (WrapMode)Enum.ToObject(typeof(WrapMode), nIntValue);
            }
            else if (strElement == "FontGradientData1_Enable")
            {
                FontGradientData[0].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "FontGradientData1_Color")
            {
                FontGradientData[0].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "FontGradientData1_Position")
            {
                FontGradientData[0].nPosition = nIntValue;
            }
            else if (strElement == "FontGradientData2_Enable")
            {
                FontGradientData[1].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "FontGradientData2_Color")
            {
                FontGradientData[1].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "FontGradientData2_Position")
            {
                FontGradientData[1].nPosition = nIntValue;
            }
            else if (strElement == "FontGradientData3_Enable")
            {
                FontGradientData[2].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "FontGradientData3_Color")
            {
                FontGradientData[2].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "FontGradientData3_Position")
            {
                FontGradientData[2].nPosition = nIntValue;
            }
            else if (strElement == "FontGradientData4_Enable")
            {
                FontGradientData[3].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "FontGradientData4_Color")
            {
                FontGradientData[3].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "FontGradientData4_Position")
            {
                FontGradientData[3].nPosition = nIntValue;
            }
            else if (strElement == "FontGradientData5_Enable")
            {
                FontGradientData[4].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "FontGradientData5_Color")
            {
                FontGradientData[4].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "FontGradientData5_Position")
            {
                FontGradientData[4].nPosition = nIntValue;
            }
            else if (strElement == "FontGradientData6_Enable")
            {
                FontGradientData[5].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "FontGradientData6_Color")
            {
                FontGradientData[5].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "FontGradientData6_Position")
            {
                FontGradientData[5].nPosition = nIntValue;
            }
            else if (strElement == "EdgeGradientWidth")
            {
                EdgeGradientWidth = nIntValue;
            }
            else if (strElement == "EdgeGradientHeight")
            {
                EdgeGradientHeight = nIntValue;
            }
            else if (strElement == "EdgeGradientAngle")
            {
                EdgeGradientAngle = nIntValue;
            }
            else if (strElement == "EdgeGradientStartX")
            {
                EdgeGradientStartX = nIntValue;
            }
            else if (strElement == "EdgeGradientStartY")
            {
                EdgeGradientStartY = nIntValue;
            }
            else if (strElement == "EdgeGradientEndX")
            {
                EdgeGradientEndX = nIntValue;
            }
            else if (strElement == "EdgeGradientEndY")
            {
                EdgeGradientEndY = nIntValue;
            }
            else if (strElement == "EdgeGradientWrapMode")
            {
                EdgeGradientWrapMode = (WrapMode)Enum.ToObject(typeof(WrapMode), nIntValue);
            }
            else if (strElement == "EdgeGradientData1_Enable")
            {
                EdgeGradientData[0].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeGradientData1_Color")
            {
                EdgeGradientData[0].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "EdgeGradientData1_Position")
            {
                EdgeGradientData[0].nPosition = nIntValue;
            }
            else if (strElement == "EdgeGradientData2_Enable")
            {
                EdgeGradientData[1].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeGradientData2_Color")
            {
                EdgeGradientData[1].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "EdgeGradientData2_Position")
            {
                EdgeGradientData[1].nPosition = nIntValue;
            }
            else if (strElement == "EdgeGradientData3_Enable")
            {
                EdgeGradientData[2].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeGradientData3_Color")
            {
                EdgeGradientData[2].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "EdgeGradientData3_Position")
            {
                EdgeGradientData[2].nPosition = nIntValue;
            }
            else if (strElement == "EdgeGradientData4_Enable")
            {
                EdgeGradientData[3].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeGradientData4_Color")
            {
                EdgeGradientData[3].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "EdgeGradientData4_Position")
            {
                EdgeGradientData[3].nPosition = nIntValue;
            }
            else if (strElement == "EdgeGradientData5_Enable")
            {
                EdgeGradientData[4].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeGradientData5_Color")
            {
                EdgeGradientData[4].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "EdgeGradientData5_Position")
            {
                EdgeGradientData[4].nPosition = nIntValue;
            }
            else if (strElement == "EdgeGradientData6_Enable")
            {
                EdgeGradientData[5].bEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "EdgeGradientData6_Color")
            {
                EdgeGradientData[5].GradientColor = HTMLToColor(strValue);
            }
            else if (strElement == "EdgeGradientData6_Position")
            {
                EdgeGradientData[5].nPosition = nIntValue;
            }
            else if (strElement == "XMLFixWidthEnable")
            {
                XMLFixWidthEnable = Convert.ToBoolean(strValue);
            }
            else if (strElement == "XMLFixWidth")
            {
                XMLFixWidth = nIntValue;
            }
            else if (strElement == "XMLUReverse")
            {
                XMLUReverse = Convert.ToBoolean(strValue);
            }
            else if (strElement == "XMLVReverse")
            {
                XMLVReverse = Convert.ToBoolean(strValue);
            }
            else if (strElement == "XMLEx")
            {
                XMLEx = Convert.ToBoolean(strValue);
            }
            else if (strElement == "HankakuKigou")
            {
                HankakuKigou = Convert.ToBoolean(strValue);
            }
            else if (strElement == "HankakuNumber")
            {
                HankakuNumber = Convert.ToBoolean(strValue);
            }
            else if (strElement == "HankakuAlphabet")
            {
                HankakuAlphabet = Convert.ToBoolean(strValue);
            }
            else if (strElement == "HankakuKatakana")
            {
                HankakuKatakana = Convert.ToBoolean(strValue);
            }
            else if (strElement == "ZenkakuKigou")
            {
                ZenkakuKigou = Convert.ToBoolean(strValue);
            }
            else if (strElement == "ZenkakuNumber")
            {
                ZenkakuNumber = Convert.ToBoolean(strValue);
            }
            else if (strElement == "ZenkakuAlphabet")
            {
                ZenkakuAlphabet = Convert.ToBoolean(strValue);
            }
            else if (strElement == "ZenkakuHiragana")
            {
                ZenkakuHiragana = Convert.ToBoolean(strValue);
            }
            else if (strElement == "ZenkakuKatakana")
            {
                ZenkakuKatakana = Convert.ToBoolean(strValue);
            }
            else if (strElement == "ZenkakuRussian")
            {
                ZenkakuRussian = Convert.ToBoolean(strValue);
            }
            else if (strElement == "ZenkakuLine")
            {
                ZenkakuLine = Convert.ToBoolean(strValue);
            }
            else if (strElement == "ZenkakuOthers")
            {
                ZenkakuOthers = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiExists")
            {
                KanjiExists = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiElementary1")
            {
                KanjiElementary1 = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiElementary2")
            {
                KanjiElementary2 = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiElementary3")
            {
                KanjiElementary3 = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiElementary4")
            {
                KanjiElementary4 = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiElementary5")
            {
                KanjiElementary5 = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiElementary6")
            {
                KanjiElementary6 = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiMiddle")
            {
                KanjiMiddle = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiName")
            {
                KanjiName = Convert.ToBoolean(strValue);
            }
            else if (strElement == "KanjiOther")
            {
                KanjiOther = Convert.ToBoolean(strValue);
            }
            else if (strElement == "OthersExists")
            {
                OthersExists = Convert.ToBoolean(strValue);
            }
            else if (strElement == "OthersList")
            {
                OthersList = strValue;
            }
        }

        // カラーコンバート関数(ColorTranslator.FromHtml()及び、ColorTranslator.ToHtml()のα値対応版)

        public static Color HTMLToColor(string strHTML)
        {
            int A, R, G, B;

            // 形式チェック
            if (strHTML.Length != 9)
            {
                return Color.FromArgb(0, 0, 0, 0);
            }
            if (strHTML[0] != '#')
            {
                return Color.FromArgb(0, 0, 0, 0);
            }
            for (int n = 1; n <= 8; n++)
            {
                if (!Uri.IsHexDigit(strHTML[n]))
                {
                    return Color.FromArgb(0, 0, 0, 0);
                }
            }

            A = Convert.ToInt32(strHTML.Substring(1, 2), 16);
            R = Convert.ToInt32(strHTML.Substring(3, 2), 16);
            G = Convert.ToInt32(strHTML.Substring(5, 2), 16);
            B = Convert.ToInt32(strHTML.Substring(7, 2), 16);

            return Color.FromArgb(A, R, G, B);
        }

        public static string ColorToHTML(Color c)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }
    }
}
