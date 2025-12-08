//////////////////////////////////////////////////////////////////////////////
//
// XMLTagWriterClass : 文書管理用タグXML作成クラス
//
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

//////////////////////////////////////////////////////////
// XMLTagWriterClass
//////////////////////////////////////////////////////////

public partial class XMLTagWriterClass : IDisposable
{
    //////////////////////////////////////////////////////////
    // 定数
    //////////////////////////////////////////////////////////

    const string NAMESPACE_ATTR = "xmlns";

    //////////////////////////////////////////////////////////
    // メンバ
    //////////////////////////////////////////////////////////

    private XmlTextWriter Writer;

    //////////////////////////////////////////////////////////
    // コンストラクタ
    //////////////////////////////////////////////////////////

    public XMLTagWriterClass(string Filename)
    {
        Writer = new XmlTextWriter(Filename, new UTF8Encoding(false));

        Writer.Formatting = Formatting.Indented;
        Writer.IndentChar = ' ';
        Writer.Indentation = 4;

        Writer.WriteStartDocument();
    }

    //////////////////////////////////////////////////////////
    // デストラクタ
    //////////////////////////////////////////////////////////

    ~XMLTagWriterClass()
    {
        Dispose();
    }

    //////////////////////////////////////////////////////////
    // Dispose()
    //////////////////////////////////////////////////////////

    public void Dispose()
    {
        if (Writer != null)
        {
            Writer.WriteEndDocument();
            Writer.Close();
            Writer = null;
        }
    }
}

//////////////////////////////////////////////////////////
// 関数
//
// Close() : 全ての処理を終了する
//
// BeginElement()   : タグを開始し、pushする
// EndElement()     : タグを終了し、popする
// WriteElenemt()   : タグと値情報を同時に書き込む
//
// WriteAttribute() : 属性情報を書き込む
// WriteValue()     : 値情報を書き込む
//////////////////////////////////////////////////////////

public partial class XMLTagWriterClass
{
    public void Close()
    {
        Dispose();
    }

    // タグを開始し、pushする
    public void BeginElement(string strElement)
    {
        Writer.WriteStartElement(strElement);
    }

    // タグを開始し、pushする(名前空間を指定)
    public void BeginElement(string strElement, string strNamespace)
    {
        Writer.WriteStartElement(strElement);
        WriteAttribute(NAMESPACE_ATTR, strNamespace);
    }

    // タグを開始し、属性を一つ記述してpushする
    public void BeginElement(string strElement, string strAttr, string strValue)
    {
        BeginElement(strElement);
        WriteAttribute(strAttr, strValue);
    }

    // タグと値を書き込む
    public void WriteElement(string strElement, string strValue)
    {
        BeginElement(strElement);
        WriteValue(strValue);
        EndElement();
    }

    // タグと属性と値を書き込む
    public void WriteElement(string strElement, string strAttr, string strAttrValue, string strValue)
    {
        BeginElement(strElement, strAttr, strAttrValue);
        WriteValue(strValue);
        EndElement();
    }

    // タグと２つの属性と値を書き込む
    public void WriteElement(string strElement, string strAttr1, string strAttrValue1
        , string strAttr2, string strAttrValue2, string strValue)
    {
        BeginElement(strElement);
        WriteAttribute(strAttr1, strAttrValue1);
        WriteAttribute(strAttr2, strAttrValue2);
        WriteValue(strValue);
        EndElement();
    }

    // タグを終了してpopする
    public void EndElement()
    {
        Writer.WriteEndElement();
    }

    // 追加の属性を記述する。
    public void WriteAttribute(string strAttr, string strValue)
    {
        Writer.WriteStartAttribute(strAttr);
        Writer.WriteString(strValue);
        Writer.WriteEndAttribute();
    }

    // 値を書き込む
    public void WriteValue(string strValue)
    {
        Writer.WriteString(strValue);
    }
}
