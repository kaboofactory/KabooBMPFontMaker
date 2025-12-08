using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public struct PointSetting
{
    public bool bEnable;
    public Color GradientColor;
    public int nPosition;
};

namespace BMPFontMaker
{
    public partial class SetLinearGradientBrushForm : Form
    {
        // ピクチャーボックス用イメージ
        private Bitmap m_bmpPixtureBox;

        // サンプル表示用ブラシ
        private LinearGradientBrush m_Brush;

        // インターフェース用公開変数
        public const int BrushDataMaxCount = 6;
        public int BrushWidth, BrushHeight, BrushAngle, BrushStartX, BrushStartY, BrushEndX, BrushEndY;
        public PointSetting[] BrushData = new PointSetting[BrushDataMaxCount];
        public WrapMode BrushWrapMode;

        public SetLinearGradientBrushForm()
        {
            InitializeComponent();
            m_Brush = null;
            m_bmpPixtureBox = null;

            // 公開データ初期化
            BrushWidth = 64;
            BrushHeight = 64;
            BrushAngle = 0;
            for (int n = 0; n < BrushDataMaxCount; n++)
            {
                BrushData[n].bEnable = false;
                if (n != BrushDataMaxCount - 1)
                {
                    BrushData[n].GradientColor = Color.FromArgb(255, 0, 0, 0);
                }
                else
                {
                    BrushData[n].GradientColor = Color.FromArgb(255, 255, 255, 255);
                }
                BrushData[n].nPosition = n * 20;
            }
            BrushStartX = 0;
            BrushStartY = 0;
            BrushEndX = 0;
            BrushEndY = 100;
            BrushWrapMode = WrapMode.Tile;
        }

        private void SetLinearGradientBrushForm_Shown(object sender, EventArgs e)
        {
            // 最初に表示された時のイベント。
            // 設定値をもとにコンポーネントの値を設定する

            // ブラシ幅・高さ
            textBoxBrushWidth.Text = BrushWidth.ToString();
            textBoxBrushHeight.Text = BrushHeight.ToString();

            // 描画角度
            trackBarAngle.Value = BrushAngle;

            // グラデーション始点・終点座標
            trackBarX1.Value = BrushStartX;
            trackBarY1.Value = BrushStartY;
            trackBarX2.Value = BrushEndX;
            trackBarY2.Value = BrushEndY;

            // Webカラー
            textBoxStartColor.Text = Data.ColorToHTML(BrushData[0].GradientColor);
            textBoxM1.Text = Data.ColorToHTML(BrushData[1].GradientColor);
            textBoxM2.Text = Data.ColorToHTML(BrushData[2].GradientColor);
            textBoxM3.Text = Data.ColorToHTML(BrushData[3].GradientColor);
            textBoxM4.Text = Data.ColorToHTML(BrushData[4].GradientColor);
            textBoxEndColor.Text = Data.ColorToHTML(BrushData[BrushDataMaxCount - 1].GradientColor);

            // 位置
            trackBarM1.Value = BrushData[1].nPosition;
            trackBarM2.Value = BrushData[2].nPosition;
            trackBarM3.Value = BrushData[3].nPosition;
            trackBarM4.Value = BrushData[4].nPosition;

            // 使用フラグ
            checkBoxM1.Checked = BrushData[1].bEnable;
            checkBoxM2.Checked = BrushData[2].bEnable;
            checkBoxM3.Checked = BrushData[3].bEnable;
            checkBoxM4.Checked = BrushData[4].bEnable;

            // ボタン色
            buttonCBStart.BackColor = BrushData[0].GradientColor;
            buttonCB1.BackColor = BrushData[1].GradientColor;
            buttonCB2.BackColor = BrushData[2].GradientColor;
            buttonCB3.BackColor = BrushData[3].GradientColor;
            buttonCB4.BackColor = BrushData[4].GradientColor;
            buttonCBEnd.BackColor = BrushData[BrushDataMaxCount - 1].GradientColor;

            // ラップモード
            switch (BrushWrapMode)
            {
                case WrapMode.Tile:
                    radioButtonTile.Checked = true;
                    break;
                case WrapMode.TileFlipX:
                    radioButtonTileX.Checked = true;
                    break;
                case WrapMode.TileFlipY:
                    radioButtonTileY.Checked = true;
                    break;
                case WrapMode.TileFlipXY:
                    radioButtonTileXY.Checked = true;
                    break;
            }

            // プレビュー更新
            buttonPreview_Click(this, new EventArgs());
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // OKを返して終了する

        }

        private void trackBarX1_ValueChanged(object sender, EventArgs e)
        {
            // 始点グラデーションX座標
            BrushStartX = trackBarX1.Value;
            labelX1.Text = String.Format("{0:F2}", (double)BrushStartX / 100.0);
        }

        private void trackBarY1_ValueChanged(object sender, EventArgs e)
        {
            // 始点グラデーションY座標
            BrushStartY = trackBarY1.Value;
            labelY1.Text = String.Format("{0:F2}", (double)BrushStartY / 100.0);
        }

        private void trackBarX2_ValueChanged(object sender, EventArgs e)
        {
            // 終点グラデーションX座標
            BrushEndX = trackBarX2.Value;
            labelX2.Text = String.Format("{0:F2}", (double)BrushEndX / 100.0);
        }

        private void trackBarY2_ValueChanged(object sender, EventArgs e)
        {
            // 終点グラデーションY座標
            BrushEndY = trackBarY2.Value;
            labelY2.Text = String.Format("{0:F2}", (double)BrushEndY / 100.0);
        }

        private void trackBarM1_ValueChanged(object sender, EventArgs e)
        {
            // 中間点1位置
            BrushData[1].nPosition = trackBarM1.Value;
            labelM1.Text = String.Format("{0:F2}", (double)BrushData[1].nPosition / 100.0);
        }

        private void trackBarM2_ValueChanged(object sender, EventArgs e)
        {
            // 中間点2位置
            BrushData[2].nPosition = trackBarM2.Value;
            labelM2.Text = String.Format("{0:F2}", (double)BrushData[2].nPosition / 100.0);
        }

        private void trackBarM3_ValueChanged(object sender, EventArgs e)
        {
            // 中間点3位置
            BrushData[3].nPosition = trackBarM3.Value;
            labelM3.Text = String.Format("{0:F2}", (double)BrushData[3].nPosition / 100.0);
        }

        private void trackBarM4_ValueChanged(object sender, EventArgs e)
        {
            // 中間点4位置
            BrushData[4].nPosition = trackBarM4.Value;
            labelM4.Text = String.Format("{0:F2}", (double)BrushData[4].nPosition / 100.0);
        }

        private void trackBarAngle_ValueChanged(object sender, EventArgs e)
        {
            // 角度
            BrushAngle = trackBarAngle.Value;
            labelAngle.Text = BrushAngle.ToString() + "°";
        }

        private void textBoxBrushWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 数字以外のキャンセル
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void textBoxBrushHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 数字以外のキャンセル
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void checkBoxM1_CheckedChanged(object sender, EventArgs e)
        {
            // 使用フラグ
            BrushData[1].bEnable = checkBoxM1.Checked;
        }

        private void checkBoxM2_CheckedChanged(object sender, EventArgs e)
        {
            // 使用フラグ
            BrushData[2].bEnable = checkBoxM2.Checked;
        }

        private void checkBoxM3_CheckedChanged(object sender, EventArgs e)
        {
            // 使用フラグ
            BrushData[3].bEnable = checkBoxM3.Checked;
        }

        private void checkBoxM4_CheckedChanged(object sender, EventArgs e)
        {
            // 使用フラグ
            BrushData[4].bEnable = checkBoxM4.Checked;
        }

        private void buttonWebColor_Click(object sender, EventArgs e)
        {
            // HTMLカラーを取得
            DialogResult ret;

            ret = colorDialogGradient.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                string strHTMLColor;

                strHTMLColor = Data.ColorToHTML(colorDialogGradient.Color);

                Clipboard.SetText(strHTMLColor);
            }
        }

        private void buttonCBStart_Click(object sender, EventArgs e)
        {
            colorDialogGradient.Color = BrushData[0].GradientColor;
            if(colorDialogGradient.ShowDialog() == DialogResult.OK)
            {
                textBoxStartColor.Text = Data.ColorToHTML(colorDialogGradient.Color);
                buttonCBStart.BackColor = colorDialogGradient.Color;
                BrushData[0].GradientColor = colorDialogGradient.Color;
            }
        }

        private void buttonCBEnd_Click(object sender, EventArgs e)
        {
            colorDialogGradient.Color = BrushData[BrushDataMaxCount - 1].GradientColor;
            if (colorDialogGradient.ShowDialog() == DialogResult.OK)
            {
                textBoxEndColor.Text = Data.ColorToHTML(colorDialogGradient.Color);
                buttonCBEnd.BackColor = colorDialogGradient.Color;
                BrushData[BrushDataMaxCount - 1].GradientColor = colorDialogGradient.Color;
            }
        }

        private void buttonCB1_Click(object sender, EventArgs e)
        {
            colorDialogGradient.Color = BrushData[1].GradientColor;
            if (colorDialogGradient.ShowDialog() == DialogResult.OK)
            {
                textBoxM1.Text = Data.ColorToHTML(colorDialogGradient.Color);
                buttonCB1.BackColor = colorDialogGradient.Color;
                BrushData[1].GradientColor = colorDialogGradient.Color;
            }
        }

        private void buttonCB2_Click(object sender, EventArgs e)
        {
            colorDialogGradient.Color = BrushData[2].GradientColor;
            if (colorDialogGradient.ShowDialog() == DialogResult.OK)
            {
                textBoxM2.Text = Data.ColorToHTML(colorDialogGradient.Color);
                buttonCB2.BackColor = colorDialogGradient.Color;
                BrushData[2].GradientColor = colorDialogGradient.Color;
            }
        }

        private void buttonCB3_Click(object sender, EventArgs e)
        {
            colorDialogGradient.Color = BrushData[3].GradientColor;
            if (colorDialogGradient.ShowDialog() == DialogResult.OK)
            {
                textBoxM3.Text = Data.ColorToHTML(colorDialogGradient.Color);
                buttonCB3.BackColor = colorDialogGradient.Color;
                BrushData[3].GradientColor = colorDialogGradient.Color;
            }
        }

        private void buttonCB4_Click(object sender, EventArgs e)
        {
            colorDialogGradient.Color = BrushData[4].GradientColor;
            if (colorDialogGradient.ShowDialog() == DialogResult.OK)
            {
                textBoxM4.Text = Data.ColorToHTML(colorDialogGradient.Color);
                buttonCB4.BackColor = colorDialogGradient.Color;
                BrushData[4].GradientColor = colorDialogGradient.Color;
            }
        }

        private void buttonPreview_Click(object sender, EventArgs e)
        {
            // エラーチェック
            if (BrushStartX == BrushEndX && BrushStartY == BrushEndY)
            {
                MessageBox.Show("始点と終点の位置が同一です");
                return;
            }

            // 現在の設定を元にブラシを作成する
            ColorBlend brend = new ColorBlend();
            int nCount = 0;

            for (int n = 0; n < BrushDataMaxCount; n++)
            {
                if((n == 0) || (n == BrushDataMaxCount - 1) || (BrushData[n].bEnable))
                {
                    nCount++;
                }
            }

            Color[] colors = new Color[nCount];
            float[] positions = new float[nCount];

            nCount = 0;
            for (int n = 0; n < BrushDataMaxCount; n++)
            {
                if ((n == 0) || (n == BrushDataMaxCount - 1) || (BrushData[n].bEnable))
                {
                    colors[nCount] = BrushData[n].GradientColor;
                    if (n == 0)
                    {
                        positions[nCount] = 0.0f;
                    }
                    else if (n == BrushDataMaxCount - 1)
                    {
                        positions[nCount] = 1.0f;
                    }
                    else
                    {
                        positions[nCount] = (float)BrushData[n].nPosition * 0.01f;
                    }
                    nCount++;
                }
            }

            brend.Colors = colors;
            brend.Positions = positions;

            m_Brush = new LinearGradientBrush(
                new PointF((float)BrushStartX * 0.01f * (float)BrushWidth, (float)BrushStartY * 0.01f * (float)BrushHeight)
                , new PointF((float)BrushEndX * 0.01f * (float)BrushWidth, (float)BrushEndY * 0.01f * (float)BrushHeight)
                , BrushData[0].GradientColor
                , BrushData[BrushDataMaxCount - 1].GradientColor);
            m_Brush.WrapMode = BrushWrapMode;
            m_Brush.InterpolationColors = brend;
            m_Brush.RotateTransform((float)BrushAngle);

            // 作成したブラシを描画する
            m_bmpPixtureBox = new Bitmap(BrushWidth, BrushHeight);

            Graphics g = Graphics.FromImage(m_bmpPixtureBox);
            g.FillRectangle(m_Brush, new Rectangle(0, 0, BrushWidth, BrushHeight));

            // PictureBoxにセットする
            pictureBoxSample.Image = m_bmpPixtureBox;
        }

        private void textBoxBrushWidth_Leave(object sender, EventArgs e)
        {
            int nRet;

            if (int.TryParse(textBoxBrushWidth.Text, out nRet) && nRet > 0)
            {
                BrushWidth = nRet;
            }
        }

        private void textBoxBrushHeight_Leave(object sender, EventArgs e)
        {
            int nRet;

            if (int.TryParse(textBoxBrushHeight.Text, out nRet) && nRet > 0)
            {
                BrushHeight = nRet;
            }
        }

        private void SetLinearGradientBrushForm_Load(object sender, EventArgs e)
        {

        }

        private void radioButtonTile_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTile.Checked)
            {
                BrushWrapMode = WrapMode.Tile;
            }
        }

        private void radioButtonTileX_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTileX.Checked)
            {
                BrushWrapMode = WrapMode.TileFlipX;
            }
        }

        private void radioButtonTileY_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTileY.Checked)
            {
                BrushWrapMode = WrapMode.TileFlipY;
            }
        }

        private void radioButtonTileXY_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTileXY.Checked)
            {
                BrushWrapMode = WrapMode.TileFlipXY;
            }
        }

        private void textBoxStartColor_Leave(object sender, EventArgs e)
        {
            BrushData[0].GradientColor = Data.HTMLToColor(textBoxStartColor.Text);
            textBoxStartColor.Text = Data.ColorToHTML(BrushData[0].GradientColor);
            buttonCBStart.BackColor = BrushData[0].GradientColor;
        }

        private void textBoxEndColor_Leave(object sender, EventArgs e)
        {
            BrushData[BrushDataMaxCount - 1].GradientColor = Data.HTMLToColor(textBoxEndColor.Text);
            textBoxEndColor.Text = Data.ColorToHTML(BrushData[BrushDataMaxCount - 1].GradientColor);
            buttonCBEnd.BackColor = BrushData[BrushDataMaxCount - 1].GradientColor;
        }

        private void textBoxM1_Leave(object sender, EventArgs e)
        {
            BrushData[1].GradientColor = Data.HTMLToColor(textBoxM1.Text);
            textBoxM1.Text = Data.ColorToHTML(BrushData[1].GradientColor);
            buttonCB1.BackColor = BrushData[1].GradientColor;
        }

        private void textBoxM2_Leave(object sender, EventArgs e)
        {
            BrushData[2].GradientColor = Data.HTMLToColor(textBoxM2.Text);
            textBoxM2.Text = Data.ColorToHTML(BrushData[2].GradientColor);
            buttonCB2.BackColor = BrushData[2].GradientColor;
        }

        private void textBoxM3_Leave(object sender, EventArgs e)
        {
            BrushData[3].GradientColor = Data.HTMLToColor(textBoxM3.Text);
            textBoxM3.Text = Data.ColorToHTML(BrushData[3].GradientColor);
            buttonCB3.BackColor = BrushData[3].GradientColor;
        }

        private void textBoxM4_Leave(object sender, EventArgs e)
        {
            BrushData[4].GradientColor = Data.HTMLToColor(textBoxM4.Text);
            textBoxM4.Text = Data.ColorToHTML(BrushData[4].GradientColor);
            buttonCB4.BackColor = BrushData[4].GradientColor;
        }
    }
}
