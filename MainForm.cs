using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMPFontMaker
{
    public partial class MainForm : Form
    {
        // PixtureBoxに描画するための背景イメージ
        Bitmap m_bmpPicBox;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // PixtureBoxに描画するためのイメージ
            m_bmpPicBox = new Bitmap(pictureBoxMain.Size.Width, pictureBoxMain.Size.Height);
            MakePlaidPatternImage();

            // 値初期化
            Data.BgColor = Color.FromArgb(0, 0, 0, 0);
            Data.LineColor = Color.FromArgb(255, 0, 255, 0);
            Data.ImageSize.Width = 512;
            Data.ImageSize.Height = 512;
            Data.Smooth = System.Drawing.Text.TextRenderingHint.AntiAlias;
            Data.PaddingTop = 2;
            Data.PaddingBottom = 2;
            Data.PaddingLeft = 2;
            Data.PaddingRight = 2;
            Data.DrawFontColor = colorDialogFontColor.Color;
            Data.DrawFontColorType = EnumBrushType.Solid;
            Data.DrawFontColorImage = null;
            Data.DrawFontColorImageFilename = "";
            Data.DrawFontColorImageScaling = false;
            Data.DrawFont = fontDialogMain.Font;

            Data.SaveImageFormat = EnumImageFormat.Png;

            Data.Edge = false;
            Data.EdgeAntialias = false;
            Data.EdgeColor = Color.DeepSkyBlue;
            Data.EdgePenWidth = 2.0f;
            Data.EdgeFirst = true;
            Data.EdgeOnly = false;
            Data.EdgeColorType = EnumBrushType.Solid;
            Data.EdgeColorImage = null;
            Data.EdgeColorImageFilename = "";
            Data.EdgeColorImageScaling = false;

            Data.FontGradientWidth = 64;
            Data.FontGradientHeight = 64;
            Data.FontGradientAngle = 0;
            for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
            {
                Data.FontGradientData[n].bEnable = false;
                if (n != SetLinearGradientBrushForm.BrushDataMaxCount - 1)
                {
                    Data.FontGradientData[n].GradientColor = Color.FromArgb(255, 0, 0, 0);
                }
                else
                {
                    Data.FontGradientData[n].GradientColor = Color.FromArgb(255, 255, 255, 255);
                }
                Data.FontGradientData[n].nPosition = n * 20;
            }
            Data.FontGradientStartX = 0;
            Data.FontGradientStartY = 0;
            Data.FontGradientEndX = 0;
            Data.FontGradientEndY = 100;

            Data.EdgeGradientWidth = 64;
            Data.EdgeGradientHeight = 64;
            Data.EdgeGradientAngle = 0;
            for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
            {
                Data.EdgeGradientData[n].bEnable = false;
                if (n != SetLinearGradientBrushForm.BrushDataMaxCount - 1)
                {
                    Data.EdgeGradientData[n].GradientColor = Color.FromArgb(255, 0, 0, 0);
                }
                else
                {
                    Data.EdgeGradientData[n].GradientColor = Color.FromArgb(255, 255, 255, 255);
                }
                Data.EdgeGradientData[n].nPosition = n * 20;
            }
            Data.EdgeGradientStartX = 0;
            Data.EdgeGradientStartY = 0;
            Data.EdgeGradientEndX = 0;
            Data.EdgeGradientEndY = 100;

            Data.HankakuKigou = true;
            Data.HankakuNumber = true;
            Data.HankakuAlphabet = true;
            Data.HankakuKatakana = false;

            Data.ZenkakuKigou = false;
            Data.ZenkakuNumber = false;
            Data.ZenkakuAlphabet = false;
            Data.ZenkakuHiragana = true;
            Data.ZenkakuKatakana = true;
            Data.ZenkakuRussian = false;
            Data.ZenkakuLine = false;
            Data.ZenkakuOthers = false;

            Data.KanjiExists = false;
            Data.KanjiElementary1 = false;
            Data.KanjiElementary2 = false;
            Data.KanjiElementary3 = false;
            Data.KanjiElementary4 = false;
            Data.KanjiElementary5 = false;
            Data.KanjiElementary6 = false;
            Data.KanjiMiddle = false;
            Data.KanjiName = false;

            Data.OthersExists = false;
            Data.OthersList = "";

            Data.Filepath = "";
            Data.listBitmap = new List<Bitmap>();

            // XMLオプション
            Data.XMLFixWidthEnable = false;
            Data.XMLFixWidth = (int)Data.DrawFont.Size;
            textBoxXMLFixWidth.Text = ((int)Math.Ceiling(Data.DrawFont.Size)).ToString();
            Data.XMLUReverse = false;
            Data.XMLVReverse = false;
            Data.XMLEx = true;

            // フォントサンプル更新
            FontSampleUpdate();

            // プレビュー
            buttonPreview_Click(this, new EventArgs());
        }

        private void MakePlaidPatternImage()
        {
            // 市松模様のイメージを作成する
            Graphics g = Graphics.FromImage(m_bmpPicBox);
            SolidBrush grayBrush = new SolidBrush(Color.FromArgb(191, 191, 191));

            // まず白で塗りつぶす
            g.FillRectangle(Brushes.White, 0, 0, m_bmpPicBox.Width, m_bmpPicBox.Height);

            // RGB(191,191,191)の四角(8x8)を描画する
            for (int x = 0; x < m_bmpPicBox.Width / 8; x ++)
            {
                for (int y = 0; y < m_bmpPicBox.Height / 8; y++)
                {
                    if ((x + y) % 2 == 1)
                    {
                        g.FillRectangle(grayBrush, x * 8, y * 8, 8, 8);
                    }
                }
            }

            // 解放
            g.Dispose();
        }

        private Bitmap GetPictureBoxImage(Bitmap bmpFore)
        {
            // 表示用のイメージを生成する
            Bitmap bmpNew = new Bitmap(m_bmpPicBox.Width, m_bmpPicBox.Height);
            Graphics g = Graphics.FromImage(bmpNew);

            // 市松模様のイメージをコピーする
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            g.DrawImage(m_bmpPicBox, 0, 0);

            // 表のイメージを重ねる(拡大縮小)
            g.DrawImage(bmpFore, new Rectangle(0, 0, m_bmpPicBox.Width, m_bmpPicBox.Height)
                , new Rectangle(0, 0, bmpFore.Width, bmpFore.Height), GraphicsUnit.Pixel);

            // 解放
            g.Dispose();

            return bmpNew;
        }

        private void FontSampleUpdate()
        {
            // フォントサンプル更新
            labelFontName.Text = fontDialogMain.Font.Name;
            labelFontPoint.Text = String.Format("{0:F1}", fontDialogMain.Font.SizeInPoints);
            labelFontSample.Font = fontDialogMain.Font;
            labelFontSample.ForeColor = colorDialogFontColor.Color;
            labelFontSample2.Font = fontDialogMain.Font;
            labelFontSample2.ForeColor = colorDialogFontColor.Color;
        }

        private void buttonShowFontDialog_Click(object sender, EventArgs e)
        {
            // 文字指定ダイアログの起動
            fontDialogMain.ShowDialog();
        }

        private void panelBMP_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radioButtonBGClear_CheckedChanged(object sender, EventArgs e)
        {
            // 背景色透過
            if (radioButtonBGClear.Checked)
            {
                Data.BgColor = Color.FromArgb(0, 0, 0, 0);
            }
        }

        private void radioButtonBGWhite_CheckedChanged(object sender, EventArgs e)
        {
            // 背景色白
            if (radioButtonBGWhite.Checked)
            {
                Data.BgColor = Color.FromArgb(255, 255, 255, 255);
            }
        }

        private void radioButtonBGBlack_CheckedChanged(object sender, EventArgs e)
        {
            // 背景色黒
            if (radioButtonBGBlack.Checked)
            {
                Data.BgColor = Color.FromArgb(255, 0, 0, 0);
            }
        }

        private void buttonBGColor_Click(object sender, EventArgs e)
        {
            // 背景色指定ダイアログ起動
            DialogResult Ret;

            Ret = colorDialogBGColor.ShowDialog();
            if (Ret == System.Windows.Forms.DialogResult.OK)
            {
                Data.BgColor = colorDialogBGColor.Color;
                radioButtonBGColor.Focus();
                radioButtonBGColor.Checked = true;
                panelBGColor.BackColor = colorDialogBGColor.Color;
            }
        }

        private void radioButtonBGColor_CheckedChanged(object sender, EventArgs e)
        {
            // 背景色指定色
            if (radioButtonBGColor.Checked)
            {
                Data.BgColor = colorDialogBGColor.Color;
            }
        }

        private void buttonLineColor_Click(object sender, EventArgs e)
        {
            // 背景色指定ダイアログ起動
            DialogResult Ret;

            Ret = colorDialogLineColor.ShowDialog();
            if (Ret == System.Windows.Forms.DialogResult.OK)
            {
                Data.LineColor = colorDialogLineColor.Color;
                radioButtonLineColor.Focus();
                radioButtonLineColor.Checked = true;
                panelLineColor.BackColor = colorDialogLineColor.Color;
            }
        }

        private void radioButtonLineNone_CheckedChanged(object sender, EventArgs e)
        {
            // 罫線色なし
            if (radioButtonLineNone.Checked)
            {
                Data.LineColor = Color.FromArgb(0, 0, 0, 0);
            }
        }

        private void radioButtonLineWhite_CheckedChanged(object sender, EventArgs e)
        {
            // 罫線色白
            if (radioButtonLineWhite.Checked)
            {
                Data.LineColor = Color.FromArgb(255, 255, 255, 255);
            }
        }

        private void radioButtonLineBlack_CheckedChanged(object sender, EventArgs e)
        {
            // 罫線色黒
            if (radioButtonLineBlack.Checked)
            {
                Data.LineColor = Color.FromArgb(255, 0, 0, 0);
            }
        }

        private void radioButtonLineColor_CheckedChanged(object sender, EventArgs e)
        {
            // 罫線指定色
            if (radioButtonLineColor.Checked)
            {
                Data.LineColor = colorDialogLineColor.Color;
            }
        }

        private void textBoxWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 数字以外のキャンセル
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 数字以外のキャンセル
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void buttonFont_Click(object sender, EventArgs e)
        {
            DialogResult Ret;

            Ret = fontDialogMain.ShowDialog();
            if (Ret == System.Windows.Forms.DialogResult.OK)
            {
                // フォントの指定
                Data.DrawFont = fontDialogMain.Font;

                // サンプルフォントの指定
                labelFontName.Text = fontDialogMain.Font.Name;
                labelFontPoint.Text = String.Format("{0:F1}", fontDialogMain.Font.SizeInPoints);
                labelFontSample.Font = fontDialogMain.Font;
                labelFontSample.ForeColor = colorDialogFontColor.Color;
                labelFontSample2.Font = fontDialogMain.Font;
                labelFontSample2.ForeColor = colorDialogFontColor.Color;

                // 固定幅オプションの指定
                textBoxXMLFixWidth.Text = ((int)Math.Ceiling(Data.DrawFont.Size)).ToString();
                textBoxXMLFixWidth.Focus();
                buttonFont.Focus();
            }
        }

        private void textBoxWidth_Leave(object sender, EventArgs e)
        {
            int nRet;

            // 画像サイズＸ
            if (Int32.TryParse(textBoxWidth.Text, out nRet))
            {
                Data.ImageSize.Width = nRet;
            }
            else
            {
                Data.ImageSize.Width = 512;
                textBoxWidth.Text = "512";
            }
        }

        private void textBoxHeight_Leave(object sender, EventArgs e)
        {
            int nRet;

            // 画像サイズＹ
            if (Int32.TryParse(textBoxHeight.Text, out nRet))
            {
                Data.ImageSize.Height = nRet;
            }
            else
            {
                Data.ImageSize.Height = 512;
                textBoxHeight.Text = "512";
            }
        }

        private void textBoxPaddingTop_Leave(object sender, EventArgs e)
        {
            int nRet;

            // パディングTop
            if (textBoxPaddingTop.Text.Length == 0)
            {
                return;
            }
            if (Int32.TryParse(textBoxPaddingTop.Text, out nRet))
            {
                Data.PaddingTop = nRet;
            }
            else
            {
                Data.PaddingTop = 0;
            }
        }

        private void textBoxPaddingLeft_Leave(object sender, EventArgs e)
        {
            int nRet;

            // パディングLeft
            if (textBoxPaddingLeft.Text.Length == 0)
            {
                return;
            }
            if (Int32.TryParse(textBoxPaddingLeft.Text, out nRet))
            {
                Data.PaddingLeft = nRet;
            }
            else
            {
                Data.PaddingLeft = 0;
            }
        }

        private void textBoxPaddingRight_Leave(object sender, EventArgs e)
        {
            int nRet;

            // パディングRight
            if (textBoxPaddingRight.Text.Length == 0)
            {
                return;
            }
            if (Int32.TryParse(textBoxPaddingRight.Text, out nRet))
            {
                Data.PaddingRight = nRet;
            }
            else
            {
                Data.PaddingRight = 0;
            }
        }

        private void textBoxPaddingBottom_Leave(object sender, EventArgs e)
        {
            int nRet;

            // パディングBottom
            if (textBoxPaddingBottom.Text.Length == 0)
            {
                return;
            }
            if (Int32.TryParse(textBoxPaddingBottom.Text, out nRet))
            {
                Data.PaddingBottom = nRet;
            }
            else
            {
                Data.PaddingBottom = 0;
            }
        }

        private void radioButtonSmoothAntiAlias_CheckedChanged(object sender, EventArgs e)
        {
            // フォントレンダリングヒント指定
            if (radioButtonSmoothAntiAlias.Checked)
            {
                Data.Smooth = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
        }

        private void radioButtonSmoothAntiAliasGridFit_CheckedChanged(object sender, EventArgs e)
        {
            // フォントレンダリングヒント指定
            if (radioButtonSmoothAntiAliasGridFit.Checked)
            {
                Data.Smooth = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }
        }

        private void radioButtonSmoothClearTypeGridFit_CheckedChanged(object sender, EventArgs e)
        {
            // フォントレンダリングヒント指定
            if (radioButtonSmoothClearTypeGridFit.Checked)
            {
                Data.Smooth = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            }
        }

        private void radioButtonSmoothSingleBitPerPixel_CheckedChanged(object sender, EventArgs e)
        {
            // フォントレンダリングヒント指定
            if (radioButtonSmoothSingleBitPerPixel.Checked)
            {
                Data.Smooth = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            }
        }

        private void radioButtonSmoothSingleBitPerPixelGridFit_CheckedChanged(object sender, EventArgs e)
        {
            // フォントレンダリングヒント指定
            if (radioButtonSmoothSingleBitPerPixelGridFit.Checked)
            {
                Data.Smooth = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            }
        }

        private void radioButtonSmoothSystemDefault_CheckedChanged(object sender, EventArgs e)
        {
            // フォントレンダリングヒント指定
            if (radioButtonSmoothSystemDefault.Checked)
            {
                Data.Smooth = System.Drawing.Text.TextRenderingHint.SystemDefault;
            }
        }

        private void radioButtonPNG_CheckedChanged(object sender, EventArgs e)
        {
            // PNGフォーマットで書き出し
            if (radioButtonPNG.Checked)
            {
                Data.SaveImageFormat = EnumImageFormat.Png;
            }
        }

        private void radioButtonBMP_CheckedChanged(object sender, EventArgs e)
        {
            // BMPフォーマットで書き出し
            if (radioButtonBMP.Checked)
            {
                Data.SaveImageFormat = EnumImageFormat.Bmp;
            }
        }

        private void buttonFontColor_Click(object sender, EventArgs e)
        {
            DialogResult Ret;

            Ret = colorDialogFontColor.ShowDialog();
            if (Ret == System.Windows.Forms.DialogResult.OK)
            {
                // フォント色指定
                Data.DrawFontColor = colorDialogFontColor.Color;
                panelFontColor.BackColor = colorDialogFontColor.Color;

                // サンプルフォント更新
                FontSampleUpdate();

                // ラジオボタンをチェック
                radioButtonFontColorSolid.Checked = true;
            }
        }

        private void ScrollBarPage_ValueChanged(object sender, EventArgs e)
        {
            int nCount;

            nCount = Data.listBitmap.Count;
            if (nCount > 0 && ScrollBarPage.Value < nCount)
            {
                // ページ遷移処理
                pictureBoxMain.Image = GetPictureBoxImage(Data.listBitmap[ScrollBarPage.Value]);

                labelPage.Text = String.Format("{0} / {1}", ScrollBarPage.Value, (ScrollBarPage.Maximum - ScrollBarPage.LargeChange + 1));
            }
        }

        private void textBoxEdgeSize_Leave(object sender, EventArgs e)
        {
            float fRet;

            // 縁取り幅
            if (textBoxEdgeSize.Text.Length == 0)
            {
                return;
            }

            if (float.TryParse(textBoxEdgeSize.Text, out fRet))
            {
                Data.EdgePenWidth = fRet;
            }
            else
            {
                Data.EdgePenWidth = 0.0f;
            }
        }

        private void buttonEdgeColor_Click(object sender, EventArgs e)
        {
            // 縁取り色指定ダイアログ起動
            DialogResult Ret;

            Ret = colorDialogEdgeColor.ShowDialog();
            if (Ret == System.Windows.Forms.DialogResult.OK)
            {
                Data.EdgeColor = colorDialogEdgeColor.Color;
                panelEdgeColor.BackColor = colorDialogEdgeColor.Color;

                // ラジオボタンをチェック
                radioButtonEdgeColorSolid.Checked = true;
            }
        }

        private void radioButtonEdgeOff_CheckedChanged(object sender, EventArgs e)
        {
            // 縁取りなし
            if (radioButtonEdgeOff.Checked)
            {
                Data.Edge = false;
            }
        }

        private void radioButtonEdgeOn_CheckedChanged(object sender, EventArgs e)
        {
            // 縁取りあり
            if (radioButtonEdgeOn.Checked)
            {
                Data.Edge = true;
            }
        }

        private void radioButtonSmoothingModeOff_CheckedChanged(object sender, EventArgs e)
        {
            // 縁取りアンチエイリアスなし
            if (radioButtonSmoothingModeOff.Checked)
            {
                Data.EdgeAntialias = false;
            }
        }

        private void radioButtonSmoothingModeOn_CheckedChanged(object sender, EventArgs e)
        {
            // 縁取りアンチエイリアスあり
            if (radioButtonSmoothingModeOn.Checked)
            {
                Data.EdgeAntialias = true;
            }
        }

        private void checkBoxEdgeFirst_CheckedChanged(object sender, EventArgs e)
        {
            // 縁を先に書くかどうか
            Data.EdgeFirst = checkBoxEdgeFirst.Checked;
        }

        private void checkBoxEdgeOnly_CheckedChanged(object sender, EventArgs e)
        {
            // 縁のみを描くかどうか
            Data.EdgeOnly = checkBoxEdgeOnly.Checked;
        }

        private void radioButtonFontColorSolid_CheckedChanged(object sender, EventArgs e)
        {
            // フォント指定色(Solid)
            if (radioButtonFontColorSolid.Checked)
            {
                Data.DrawFontColorType = EnumBrushType.Solid;
            }
        }

        private void radioButtonFontColorImage_CheckedChanged(object sender, EventArgs e)
        {
            // フォント指定色(Image)
            if (radioButtonFontColorImage.Checked)
            {
                if (Data.DrawFontColorImage == null)
                {
                    buttonFontColorImage_Click(this, new EventArgs());
                }
                else
                {
                    Data.DrawFontColorType = EnumBrushType.Image;
                }
            }
        }

        private void radioButtonFontColorGradient_CheckedChanged(object sender, EventArgs e)
        {
            // フォント指定色(Gradient)
            if (radioButtonFontColorGradient.Checked)
            {
                Data.DrawFontColorType = EnumBrushType.Gradient;
            }
        }

        private void radioButtonEdgeColorSolid_CheckedChanged(object sender, EventArgs e)
        {
            // 縁取り指定色(Solid)
            if (radioButtonEdgeColorSolid.Checked)
            {
                Data.EdgeColorType = EnumBrushType.Solid;
            }
        }

        private void radioButtonEdgeColorImage_CheckedChanged(object sender, EventArgs e)
        {
            // 縁取り指定色(Image)
            if (radioButtonEdgeColorImage.Checked)
            {
                if (Data.EdgeColorImage == null)
                {
                    buttonEdgeColorImage_Click(this, new EventArgs());
                }
            }
        }

        private void radioButtonEdgeColorGradient_CheckedChanged(object sender, EventArgs e)
        {
            // 縁取り指定色(Gradient)
            if (radioButtonEdgeColorGradient.Checked)
            {
                Data.EdgeColorType = EnumBrushType.Gradient;
            }
        }

        private void buttonFontColorImage_Click(object sender, EventArgs e)
        {
            // フォント指定色(Image)ファイル名取得
            DialogResult ret;

            ret = openFileDialogFontColorImage.ShowDialog();

            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                string strFilename;

                // ファイル名を取得
                strFilename = openFileDialogFontColorImage.FileName;

                // イメージを開く
                try
                {
                    Data.DrawFontColorImage = new Bitmap(strFilename);
                }
                catch (Exception ex)
                {
                    // 例外発生
                    MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                // ファイル名を保存
                Data.DrawFontColorImageFilename = strFilename;

                // ラジオボタンをチェックする
                if (radioButtonFontColorImage.Checked == false)
                {
                    radioButtonFontColorImage.Checked = true;
                }

                // タイプを設定
                Data.DrawFontColorType = EnumBrushType.Image;
            }
            else
            {
                // Solidに戻す
                radioButtonFontColorSolid.Checked = true;
            }
        }

        private void buttonEdgeColorImage_Click(object sender, EventArgs e)
        {
            // 縁取り指定色(Image)ファイル名取得
            DialogResult ret;

            ret = openFileDialogEdgeColorImage.ShowDialog();

            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                string strFilename;

                // ファイル名を取得
                strFilename = openFileDialogEdgeColorImage.FileName;

                // イメージを開く
                try
                {
                    Data.EdgeColorImage = new Bitmap(strFilename);
                }
                catch (Exception ex)
                {
                    // 例外発生
                    MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                // ファイル名を保存
                Data.EdgeColorImageFilename = strFilename;

                // ラジオボタンをチェックする
                if (radioButtonEdgeColorImage.Checked == false)
                {
                    radioButtonEdgeColorImage.Checked = true;
                }

                // タイプを設定
                Data.EdgeColorType = EnumBrushType.Image;
            }
            else
            {
                // Solidに戻す
                radioButtonFontColorSolid.Checked = true;
            }
        }

        private void buttonFontColorGradient_Click(object sender, EventArgs e)
        {
            DialogResult ret;
            SetLinearGradientBrushForm form = new SetLinearGradientBrushForm();

            // 現状の設定を反映
            form.BrushWidth = Data.FontGradientWidth;
            form.BrushHeight = Data.FontGradientHeight;
            form.BrushAngle = Data.FontGradientAngle;
            form.BrushStartX = Data.FontGradientStartX;
            form.BrushStartY = Data.FontGradientStartY;
            form.BrushEndX = Data.FontGradientEndX;
            form.BrushEndY = Data.FontGradientEndY;
            form.BrushWrapMode = Data.FontGradientWrapMode;
            for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
            {
                form.BrushData[n].bEnable = Data.FontGradientData[n].bEnable;
                form.BrushData[n].GradientColor = Data.FontGradientData[n].GradientColor;
                form.BrushData[n].nPosition = Data.FontGradientData[n].nPosition;
            }

            // フォームを表示
            ret = form.ShowDialog();

            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                radioButtonFontColorGradient.Checked = true;

                // 設定内容を戻す
                Data.FontGradientWidth = form.BrushWidth;
                Data.FontGradientHeight = form.BrushHeight;
                Data.FontGradientAngle = form.BrushAngle;
                Data.FontGradientStartX = form.BrushStartX;
                Data.FontGradientStartY = form.BrushStartY;
                Data.FontGradientEndX = form.BrushEndX;
                Data.FontGradientEndY = form.BrushEndY;
                Data.FontGradientWrapMode = form.BrushWrapMode;
                for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
                {
                    Data.FontGradientData[n].bEnable = form.BrushData[n].bEnable;
                    Data.FontGradientData[n].GradientColor = form.BrushData[n].GradientColor;
                    Data.FontGradientData[n].nPosition = form.BrushData[n].nPosition;
                }
                form.Dispose();
            }
            else
            {
                // Solidに戻す
                radioButtonFontColorSolid.Checked = true;
            }
        }

        private void buttonEdgeColorGradient_Click(object sender, EventArgs e)
        {
            DialogResult ret;
            SetLinearGradientBrushForm form = new SetLinearGradientBrushForm();

            // 現状の設定を反映
            form.BrushWidth = Data.EdgeGradientWidth;
            form.BrushHeight = Data.EdgeGradientHeight;
            form.BrushAngle = Data.EdgeGradientAngle;
            form.BrushStartX = Data.EdgeGradientStartX;
            form.BrushStartY = Data.EdgeGradientStartY;
            form.BrushEndX = Data.EdgeGradientEndX;
            form.BrushEndY = Data.EdgeGradientEndY;
            form.BrushWrapMode = Data.EdgeGradientWrapMode;
            for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
            {
                form.BrushData[n].bEnable = Data.EdgeGradientData[n].bEnable;
                form.BrushData[n].GradientColor = Data.EdgeGradientData[n].GradientColor;
                form.BrushData[n].nPosition = Data.EdgeGradientData[n].nPosition;
            }

            // フォームを表示
            ret = form.ShowDialog();

            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                radioButtonEdgeColorGradient.Checked = true;

                // 設定内容を戻す
                Data.EdgeGradientWidth = form.BrushWidth;
                Data.EdgeGradientHeight = form.BrushHeight;
                Data.EdgeGradientAngle = form.BrushAngle;
                Data.EdgeGradientStartX = form.BrushStartX;
                Data.EdgeGradientStartY = form.BrushStartY;
                Data.EdgeGradientEndX = form.BrushEndX;
                Data.EdgeGradientEndY = form.BrushEndY;
                Data.EdgeGradientWrapMode = form.BrushWrapMode;
                for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
                {
                    Data.EdgeGradientData[n].bEnable = form.BrushData[n].bEnable;
                    Data.EdgeGradientData[n].GradientColor = form.BrushData[n].GradientColor;
                    Data.EdgeGradientData[n].nPosition = form.BrushData[n].nPosition;
                }
                form.Dispose();
            }
            else
            {
                // Solidに戻す
                radioButtonEdgeColorSolid.Checked = true;
            }
        }

        private void buttonPreview_Click(object sender, EventArgs e)
        {
            // ビットマップのプレビュー
            CreateBMPFont bmp = new CreateBMPFont();

            // 出力
            if (bmp.Create("", false) && Data.listBitmap.Count > 0)
            {
                // 出力した結果を反映
                ScrollBarPage.Maximum = Data.listBitmap.Count + ScrollBarPage.LargeChange - 2;
                ScrollBarPage.Minimum = 0;
                ScrollBarPage.Value = 0;

                labelPage.Text = String.Format("{0} / {1}", ScrollBarPage.Value, (ScrollBarPage.Maximum - ScrollBarPage.LargeChange + 1));
                pictureBoxMain.Image = GetPictureBoxImage(Data.listBitmap[ScrollBarPage.Value]);
            }
        }

        private void checkBoxXMLFixWidth_CheckedChanged(object sender, EventArgs e)
        {
            // フォント幅固定
            Data.XMLFixWidthEnable = checkBoxXMLFixWidth.Checked;
        }

        private void textBoxXMLFixWidth_Leave(object sender, EventArgs e)
        {
            int nRet;

            // 画像サイズＸ
            if (int.TryParse(textBoxXMLFixWidth.Text, out nRet))
            {
                Data.XMLFixWidth = nRet;
            }
            else
            {
                Data.XMLFixWidth = (int)Data.DrawFont.Size;
                textBoxXMLFixWidth.Text = Data.XMLFixWidth.ToString();
            }
        }

        private void textBoxXMLFixWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 数字以外のキャンセル
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void textBoxEdgeSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 数字以外のキャンセル
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void checkBoxXMLUReverse_CheckedChanged(object sender, EventArgs e)
        {
            // U値反転
            Data.XMLUReverse = checkBoxXMLUReverse.Checked;
        }

        private void checkBoxXMLVReverse_CheckedChanged(object sender, EventArgs e)
        {
            // V値反転
            Data.XMLVReverse = checkBoxXMLVReverse.Checked;
        }

        private void checkBoxXMLEx_CheckedChanged(object sender, EventArgs e)
        {
            // 拡張情報出力
            Data.XMLEx = checkBoxXMLEx.Checked;
        }

        private void checkBoxFontColorImageScaling_CheckedChanged(object sender, EventArgs e)
        {
            // ブラシイメージの自動拡大縮小
            Data.DrawFontColorImageScaling = checkBoxFontColorImageScaling.Checked;
        }

        private void checkBoxEdgeColorImageScaling_CheckedChanged(object sender, EventArgs e)
        {
            // 縁取りイメージの自動拡大縮小
            Data.EdgeColorImageScaling = checkBoxEdgeColorImageScaling.Checked;
        }

        private void MenuFileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // テキストボックスの設定を(無理やり)確定させる
            groupBox1.Focus();
        }

        private void MenuOpenRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ret;

            ret = openFileDialogMain.ShowDialog();

            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                // ファイルを開く
                Data.LoadFile(openFileDialogMain.FileName);

                //////////////////////////////////
                // 開いた設定の通りに表示する
                //////////////////////////////////

                // スムージング
                switch (Data.Smooth)
                {
                    case System.Drawing.Text.TextRenderingHint.AntiAlias:
                        radioButtonSmoothAntiAlias.Checked = true;
                        break;
                    case System.Drawing.Text.TextRenderingHint.AntiAliasGridFit:
                        radioButtonSmoothAntiAliasGridFit.Checked = true;
                        break;
                    case System.Drawing.Text.TextRenderingHint.ClearTypeGridFit:
                        radioButtonSmoothClearTypeGridFit.Checked = true;
                        break;
                    case System.Drawing.Text.TextRenderingHint.SingleBitPerPixel:
                        radioButtonSmoothSingleBitPerPixel.Checked = true;
                        break;
                    case System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit:
                        radioButtonSmoothSingleBitPerPixelGridFit.Checked = true;
                        break;
                    case System.Drawing.Text.TextRenderingHint.SystemDefault:
                        radioButtonSmoothSystemDefault.Checked = true;
                        break;
                }

                // 背景色
                if (Data.BgColor == Color.FromArgb(0, 0, 0, 0))
                {
                    radioButtonBGClear.Checked = true;
                }
                else if (Data.BgColor == Color.FromArgb(255, 255, 255, 255))
                {
                    radioButtonBGWhite.Checked = true;
                }
                else if (Data.BgColor == Color.FromArgb(255, 0, 0, 0))
                {
                    radioButtonBGBlack.Checked = true;
                }
                else
                {
                    colorDialogBGColor.Color = Data.BgColor;
                    panelBGColor.BackColor = Data.BgColor;
                    radioButtonBGColor.Checked = true;
                }

                // 生成イメージサイズ
                textBoxWidth.Text = Data.ImageSize.Width.ToString();
                textBoxHeight.Text = Data.ImageSize.Height.ToString();

                // 罫線
                if (Data.LineColor == Color.FromArgb(0, 0, 0, 0))
                {
                    radioButtonLineNone.Checked = true;
                }
                else if (Data.LineColor == Color.FromArgb(255, 255, 255, 255))
                {
                    radioButtonLineWhite.Checked = true;
                }
                else if (Data.LineColor == Color.FromArgb(255, 0, 0, 0))
                {
                    radioButtonLineBlack.Checked = true;
                }
                else
                {
                    colorDialogLineColor.Color = Data.LineColor;
                    panelLineColor.BackColor = Data.LineColor;
                    radioButtonLineColor.Checked = true;
                }

                // パディング
                textBoxPaddingLeft.Text = Data.PaddingLeft.ToString();
                textBoxPaddingRight.Text = Data.PaddingRight.ToString();
                textBoxPaddingTop.Text = Data.PaddingTop.ToString();
                textBoxPaddingBottom.Text = Data.PaddingBottom.ToString();

                // フォント
                fontDialogMain.Font = Data.DrawFont;
                colorDialogFontColor.Color = Data.DrawFontColor;
                panelFontColor.BackColor = Data.DrawFontColor;
                switch (Data.DrawFontColorType)
                {
                    case EnumBrushType.Solid:
                        radioButtonFontColorSolid.Checked = true;
                        break;
                    case EnumBrushType.Image:
                        try
                        {
                            Data.DrawFontColorImage = new Bitmap(Data.DrawFontColorImageFilename);
                            radioButtonFontColorImage.Checked = true;
                        }
                        catch (Exception ex)
                        {
                            radioButtonFontColorSolid.Checked = true;
                            Data.DrawFontColorType = EnumBrushType.Solid;
                            MessageBox.Show(String.Format("イメージ({0})のオープンに失敗しました({1})", Data.DrawFontColorImageFilename, ex.Message), "エラー"
                                , MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case EnumBrushType.Gradient:
                        radioButtonFontColorGradient.Checked = true;
                        break;
                }
                FontSampleUpdate();

                // 出力ファイルフォーマット
                switch (Data.SaveImageFormat)
                {
                    case EnumImageFormat.Png:
                        radioButtonPNG.Checked = true;
                        break;
                    case EnumImageFormat.Bmp:
                        radioButtonBMP.Checked = true;
                        break;
                }

                // XMLオプション
                if (Data.XMLFixWidthEnable)
                {
                    checkBoxXMLFixWidth.Checked = true;
                    textBoxXMLFixWidth.Text = Data.XMLFixWidth.ToString();
                }
                else
                {
                    checkBoxXMLFixWidth.Checked = false;
                    textBoxXMLFixWidth.Text = ((int)Math.Ceiling(Data.DrawFont.Size)).ToString();
                }
                checkBoxXMLUReverse.Checked = Data.XMLUReverse;
                checkBoxXMLVReverse.Checked = Data.XMLVReverse;
                checkBoxXMLEx.Checked = Data.XMLEx;

                // 縁取り
                if (Data.Edge)
                {
                    radioButtonEdgeOn.Checked = true;
                }
                else
                {
                    radioButtonEdgeOff.Checked = true;
                }
                if (Data.EdgeAntialias)
                {
                    radioButtonSmoothingModeOn.Checked = true;
                }
                else
                {
                    radioButtonSmoothingModeOff.Checked = true;
                }
                if (Data.EdgeFirst)
                {
                    checkBoxEdgeFirst.Checked = true;
                }
                else
                {
                    checkBoxEdgeFirst.Checked = false;
                }
                if (Data.EdgeOnly)
                {
                    checkBoxEdgeOnly.Checked = true;
                }
                else
                {
                    checkBoxEdgeOnly.Checked = false;
                }
                panelEdgeColor.BackColor = Data.EdgeColor;
                colorDialogEdgeColor.Color = Data.EdgeColor;
                switch (Data.EdgeColorType)
                {
                    case EnumBrushType.Solid:
                        radioButtonEdgeColorSolid.Checked = true;
                        break;
                    case EnumBrushType.Image:
                        try
                        {
                            Data.EdgeColorImage = new Bitmap(Data.EdgeColorImageFilename);
                            radioButtonEdgeColorImage.Checked = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("イメージ({0})のオープンに失敗しました({1})", Data.EdgeColorImageFilename, ex.Message), "エラー"
                                , MessageBoxButtons.OK, MessageBoxIcon.Error);
                            radioButtonEdgeColorSolid.Checked = true;
                            Data.EdgeColorType = EnumBrushType.Solid;
                        }
                        break;
                    case EnumBrushType.Gradient:
                        radioButtonEdgeColorGradient.Checked = true;
                        break;
                }
                textBoxEdgeSize.Text = String.Format("{0:F2}", Data.EdgePenWidth);

                //////////////////////////////////
                // 後処理
                //////////////////////////////////

                // イメージファイルを開く
                if (Data.DrawFontColorType == EnumBrushType.Image)
                {
                    try
                    {
                        Data.DrawFontColorImage = new Bitmap(Data.DrawFontColorImageFilename);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0}の読み込みに失敗しました。単色ブラシに変更します。{1}", Data.DrawFontColorImageFilename, ex.Message)
                            , "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Data.DrawFontColorType = EnumBrushType.Solid;
                    }
                }

                if (Data.EdgeColorType == EnumBrushType.Image)
                {
                    try
                    {
                        Data.EdgeColorImage = new Bitmap(Data.EdgeColorImageFilename);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0}の読み込みに失敗しました。単色ブラシに変更します。{1}", Data.EdgeColorImageFilename, ex.Message)
                            , "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Data.DrawFontColorType = EnumBrushType.Solid;
                    }
                }

                // プレビューする
                buttonPreview_Click(this, new EventArgs());
            }
        }

        private void MenuSaveSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Data.Filepath.Length > 0)
            {
                // 保存
                Data.SaveFile();
            }
            else
            {
                // 名前を付けて保存
                DialogResult Ret;

                Ret = saveFileDialogMain.ShowDialog();
                if (Ret == System.Windows.Forms.DialogResult.OK)
                {
                    Data.SaveFile(saveFileDialogMain.FileName);
                }
            }
        }

        private void MenuNameSaveAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 名前を付けて保存
            DialogResult Ret;

            Ret = saveFileDialogMain.ShowDialog();
            if (Ret == System.Windows.Forms.DialogResult.OK)
            {
                Data.SaveFile(saveFileDialogMain.FileName);
            }
        }

        private void OutputEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ret;

            // ビットマップの出力
            ret = saveFileDialogBMP.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                CreateBMPFont bmp = new CreateBMPFont();

                // 出力
                if (bmp.Create(saveFileDialogBMP.FileName, true) && Data.listBitmap.Count > 0)
                {
                    // 出力した結果を反映
                    ScrollBarPage.Maximum = Data.listBitmap.Count + ScrollBarPage.LargeChange - 2;
                    ScrollBarPage.Minimum = 0;
                    ScrollBarPage.Value = 0;

                    labelPage.Text = String.Format("{0} / {1}", ScrollBarPage.Value, (ScrollBarPage.Maximum - ScrollBarPage.LargeChange + 1));
                    pictureBoxMain.Image = GetPictureBoxImage(Data.listBitmap[ScrollBarPage.Value]);
                }
            }
        }

        private void ExitXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CharSetMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMojiSet form = new FormMojiSet();

            form.Show();
        }

        private void VerAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // バージョン情報の表示
            System.Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            //結果の表示
            MessageBox.Show(ver.ToString() + "かもしれない");
        }
    }
}
