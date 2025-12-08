using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMPFontMaker
{
    public partial class FormMojiSet : Form
    {
        public FormMojiSet()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // 何もせずに閉じる
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // 指定を反映させる
            Data.HankakuKigou = checkBoxHankakuKigou.Checked;
            Data.HankakuNumber = checkBoxHankakuNumber.Checked;
            Data.HankakuAlphabet = checkBoxHankakuAlphabet.Checked;
            Data.HankakuKatakana = checkBoxHankakuKatakana.Checked;

            // 全角
            Data.ZenkakuKigou = checkBoxZenkakuKigou.Checked;
            Data.ZenkakuNumber = checkBoxZenkakuNumber.Checked;
            Data.ZenkakuAlphabet = checkBoxZenkakuAlphabet.Checked;
            Data.ZenkakuHiragana = checkBoxZenkakuHiragana.Checked;
            Data.ZenkakuKatakana = checkBoxZenkakuKatakana.Checked;
            Data.ZenkakuRussian = checkBoxZenkakuRussian.Checked;
            Data.ZenkakuLine = checkBoxZenkakuLines.Checked;
            Data.ZenkakuOthers = checkBoxZenkakuOthers.Checked;

            if (radioButtonKanjiOn.Checked)
            {
                Data.KanjiExists = true;
                Data.KanjiElementary1 = checkBoxKanjiElementary1.Checked;
                Data.KanjiElementary2 = checkBoxKanjiElementary2.Checked;
                Data.KanjiElementary3 = checkBoxKanjiElementary3.Checked;
                Data.KanjiElementary4 = checkBoxKanjiElementary4.Checked;
                Data.KanjiElementary5 = checkBoxKanjiElementary5.Checked;
                Data.KanjiElementary6 = checkBoxKanjiElementary6.Checked;
                Data.KanjiMiddle = checkBoxKanjiMiddle.Checked;
                Data.KanjiName = checkBoxKanjiName.Checked;
                Data.KanjiOther = checkBoxKanjiOther.Checked;
            }
            else
            {
                Data.KanjiExists = false;
                Data.KanjiElementary1 = false;
                Data.KanjiElementary2 = false;
                Data.KanjiElementary3 = false;
                Data.KanjiElementary4 = false;
                Data.KanjiElementary5 = false;
                Data.KanjiElementary6 = false;
                Data.KanjiMiddle = false;
                Data.KanjiName = false;
                Data.KanjiOther = false;
            }

            if (radioButtonOtherOn.Checked)
            {
                Data.OthersExists = true;
                Data.OthersList = textBoxOthersList.Text;
            }
            else
            {
                Data.OthersExists = false;
                Data.OthersList = "";
            }

            Close();
        }

        private void FormMojiSet_Load(object sender, EventArgs e)
        {
            // 現在の設定を反映

            // 半角
            checkBoxHankakuKigou.Checked = Data.HankakuKigou;
            checkBoxHankakuNumber.Checked = Data.HankakuNumber;
            checkBoxHankakuAlphabet.Checked = Data.HankakuAlphabet;
            checkBoxHankakuKatakana.Checked = Data.HankakuKatakana;

            // 全角
            checkBoxZenkakuKigou.Checked = Data.ZenkakuKigou;
            checkBoxZenkakuNumber.Checked = Data.ZenkakuNumber;
            checkBoxZenkakuAlphabet.Checked = Data.ZenkakuAlphabet;
            checkBoxZenkakuHiragana.Checked = Data.ZenkakuHiragana;
            checkBoxZenkakuKatakana.Checked = Data.ZenkakuKatakana;
            checkBoxZenkakuRussian.Checked = Data.ZenkakuRussian;
            checkBoxZenkakuLines.Checked = Data.ZenkakuLine;
            checkBoxZenkakuOthers.Checked = Data.ZenkakuOthers;

            // 漢字
            if (Data.KanjiExists == true)
            {
                radioButtonKanjiOn.Checked = true;

                checkBoxKanjiElementary1.Checked = Data.KanjiElementary1;
                checkBoxKanjiElementary2.Checked = Data.KanjiElementary2;
                checkBoxKanjiElementary3.Checked = Data.KanjiElementary3;
                checkBoxKanjiElementary4.Checked = Data.KanjiElementary4;
                checkBoxKanjiElementary5.Checked = Data.KanjiElementary5;
                checkBoxKanjiElementary6.Checked = Data.KanjiElementary6;
                checkBoxKanjiMiddle.Checked = Data.KanjiMiddle;
                checkBoxKanjiName.Checked = Data.KanjiName;
                checkBoxKanjiOther.Checked = Data.KanjiOther;
            }
            else
            {
                radioButtonKanjiOff.Checked = true;
            }

            // その他
            if (Data.OthersExists == true)
            {
                radioButtonOtherOn.Checked = true;
                textBoxOthersList.Enabled = true;
                textBoxOthersList.Text = Data.OthersList;
            }
            else
            {
                radioButtonOtherOff.Checked = true;
                textBoxOthersList.Enabled = false;
            }
        }

        private void radioButtonKanjiOff_CheckedChanged(object sender, EventArgs e)
        {
            // オンになっている項目はオフにする
            if (radioButtonKanjiOff.Checked)
            {
                checkBoxKanjiElementary1.Checked = false;
                checkBoxKanjiElementary2.Checked = false;
                checkBoxKanjiElementary3.Checked = false;
                checkBoxKanjiElementary4.Checked = false;
                checkBoxKanjiElementary5.Checked = false;
                checkBoxKanjiElementary6.Checked = false;
                checkBoxKanjiMiddle.Checked = false;
                checkBoxKanjiName.Checked = false;
                checkBoxKanjiOther.Checked = false;

                checkBoxKanjiElementary1.Enabled = false;
                checkBoxKanjiElementary2.Enabled = false;
                checkBoxKanjiElementary3.Enabled = false;
                checkBoxKanjiElementary4.Enabled = false;
                checkBoxKanjiElementary5.Enabled = false;
                checkBoxKanjiElementary6.Enabled = false;
                checkBoxKanjiMiddle.Enabled = false;
                checkBoxKanjiName.Enabled = false;
                checkBoxKanjiOther.Enabled = false;
            }
        }

        private void radioButtonKanjiOn_CheckedChanged(object sender, EventArgs e)
        {
            // オン
            if (radioButtonKanjiOn.Checked)
            {
                checkBoxKanjiElementary1.Enabled = true;
                checkBoxKanjiElementary2.Enabled = true;
                checkBoxKanjiElementary3.Enabled = true;
                checkBoxKanjiElementary4.Enabled = true;
                checkBoxKanjiElementary5.Enabled = true;
                checkBoxKanjiElementary6.Enabled = true;
                checkBoxKanjiMiddle.Enabled = true;
                checkBoxKanjiName.Enabled = true;
                checkBoxKanjiOther.Enabled = true;
            }
        }

        private void radioButtonOtherOff_CheckedChanged(object sender, EventArgs e)
        {
            // テキストボックスをdisableにする
            if (radioButtonOtherOff.Checked)
            {
                textBoxOthersList.Enabled = false;
            }
        }

        private void radioButtonOtherOn_CheckedChanged(object sender, EventArgs e)
        {
            // テキストボックスをenableにする
            if (radioButtonOtherOn.Checked)
            {
                textBoxOthersList.Enabled = true;
            }
        }

        private void checkBoxKanjiElementary1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void buttonGetUniqueChar_Click(object sender, EventArgs e)
        {
            // 各文字が一意となるテキストを抽出するためのフォームを表示する
            UniqueCharReaderForm form = new UniqueCharReaderForm();
            DialogResult ret = form.ShowDialog();

            if (ret == DialogResult.OK)
            {
                radioButtonOtherOn.Checked = true;
                textBoxOthersList.Text = form.UniqueString;
            }

            form.Dispose();
        }
    }
}
