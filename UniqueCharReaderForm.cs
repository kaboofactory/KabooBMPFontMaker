using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace BMPFontMaker
{
    public partial class UniqueCharReaderForm : Form
    {
        // 文字コード一覧
        private EncodingInfo[] mInfoList;

        // 使用している文字の一覧
        public string UniqueString;

        public UniqueCharReaderForm()
        {
            InitializeComponent();
            UniqueString = "";
        }

        private void UniqueCharReaderForm_Shown(object sender, EventArgs e)
        {
            int nUTFPos = -1;

            // 最初に表示されるとき
            mInfoList = Encoding.GetEncodings();

            foreach (EncodingInfo ei in mInfoList)
            {
               comboBoxEncodeList.Items.Add(String.Format("{0} : {1}({2})", ei.CodePage.ToString(), ei.DisplayName, ei.Name));

               if (ei.CodePage == 65001)
               {
                   nUTFPos = comboBoxEncodeList.Items.Count - 1;
               }
            }
            comboBoxEncodeList.SelectedIndex = 0;

            // UTF-8を標準の表示にする
            if (nUTFPos >= 0)
            {
                comboBoxEncodeList.SelectedIndex = nUTFPos;
            }

            // ボタンなフォーカスを合わせる
            buttonFileOpen.Focus();
        }

        private void buttonFileOpen_Click(object sender, EventArgs e)
        {
            DialogResult ret;

            ret = openFileDialogTextFile.ShowDialog();

            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                string AddUniqueString = "";

                string [] Filenames = openFileDialogTextFile.FileNames;

                for (int n = 0; n < Filenames.Length; n++)
                {
                    // UTF-16で表現できない文字を読み込まないようにテキストを読み込む
                    StreamReader sr = new StreamReader(Filenames[n], Encoding.GetEncoding(mInfoList[comboBoxEncodeList.SelectedIndex].CodePage
                        , new EncoderReplacementFallback(""), new DecoderReplacementFallback("")));

                    // 改行を削除し、重複なしの文字リストを取得する
                    AddUniqueString += new string(sr.ReadToEnd().Replace("\r", "").Replace("\n", "").ToCharArray().Distinct().ToArray());

                    sr.Close();
                    sr.Dispose();
                }

                // ファイルごとの重複なしの文字リストを取得したので、再度重複なしの文字リストを作成する
                AddUniqueString = new string(AddUniqueString.ToCharArray().Distinct().ToArray());

                // ソートする
                List<char> listChar = AddUniqueString.ToList();
                listChar.Sort();

                // テキストを保持し、表示する
                UniqueString = new string(listChar.ToArray());
                textBoxUnique.Text = UniqueString;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSaveFile_Click(object sender, EventArgs e)
        {
            DialogResult ret;

            ret = saveFileDialogText.ShowDialog();

            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                // UTF-8でテキストを保存する
                StreamWriter sw = new StreamWriter(saveFileDialogText.FileName, false
                    , Encoding.GetEncoding(Encoding.UTF8.CodePage, new EncoderReplacementFallback(""), new DecoderReplacementFallback("")));

                sw.Write(UniqueString);
                sw.Close();
                sw.Dispose();
            }
        }
    }
}
