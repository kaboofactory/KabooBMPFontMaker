//////////////////////////////////////////////////////////////////////////////
//
// XMLTagReaderClass : 文書管理用タグXML読み込みクラス
//
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

//////////////////////////////////////////////////////////
// 簡易の属性情報クラス
//////////////////////////////////////////////////////////

public class AttrInfo : IDisposable
{
    // 属性名
    public string Name;

    // 属性の値
    public string Value;

    public void Dispose()
    {
        Name = null;
        Value = null;
    }
}

//////////////////////////////////////////////////////////
// XMLTagReaderClass
//////////////////////////////////////////////////////////

public partial class XMLTagReaderClass : IDisposable
{
    //////////////////////////////////////////////////////////
    // メンバ
    //////////////////////////////////////////////////////////

    // ストリーム
    private StreamReader LocalStream;

    // XMLリーダ
    private XmlTextReader Reader;

    // ネストしたノードリスト
    private List<string> Path = new List<string>();

    // ネストカウント
    public int NestCount;
    
    // 要素の属性情報
    public List<AttrInfo> AttrValue;

    // テキスト情報
    public string Value;

    // ノードタイプ
    public XmlNodeType NodeType = XmlNodeType.None;

    // 空の要素を示す
    public bool IsEmptyElement;

    //////////////////////////////////////////////////////////
    // コンストラクタ (ファイル名とエンコード文字列を指定する)
    //////////////////////////////////////////////////////////

    public XMLTagReaderClass(string Filename): this(Filename, "utf-8")
    {
    }

    public XMLTagReaderClass(string Filename, string Encode)
    {
        LocalStream = new StreamReader(Filename, System.Text.Encoding.GetEncoding(Encode));
        Reader = new XmlTextReader(LocalStream);

        // 空文字列は無視する
        Reader.WhitespaceHandling = WhitespaceHandling.None;

        Path.Clear();
        Value = null;
        AttrValue = null;
        NestCount = 0;
    }

    //////////////////////////////////////////////////////////
    // デストラクタ
    //////////////////////////////////////////////////////////

    ~XMLTagReaderClass()
    {
        Dispose();
    }

    //////////////////////////////////////////////////////////
    // Dispose()
    //////////////////////////////////////////////////////////

    public void Dispose()
    {
        if (Reader != null)
        {
            LocalStream.Close();
            LocalStream = null;
            Reader.Close();
            Reader = null;
            Path.Clear();
            Path = null;
        }
    }
}

//////////////////////////////////////////////////////////
// Read() : 次の情報を読み込む
//
// 成功した場合、NodeTypeに
// XmlNodeType.Element または
// XmlNodeType.EndElement または
// XmlNodeType.Text をセットする。
//
// XmlNodeType.Element または
// XmlNodeType.EndElement の場合はValueに要素名をセットし、
// XmlNodeType.Text の場合はValueにテキストの内容をセットする。
//
// さらに XmlNodeType.Element の場合は、
// AttrValue に属性情報の一覧をセットする。
//////////////////////////////////////////////////////////

public partial class XMLTagReaderClass
{
    public bool Read()
    {
        if (Reader.ReadState == ReadState.Closed ||
            Reader.ReadState == ReadState.EndOfFile ||
            Reader.ReadState == ReadState.Error)
        {
            return false;
        }

        // 次の情報を読み込む
        if (Reader.Read())
        {
            IsEmptyElement = false;
            switch (Reader.NodeType)
            {
                case XmlNodeType.Element:
                    // 要素の場合

                    NodeType = Reader.NodeType;
                    Value = Reader.Name;

                    // 現在のノードが空の要素ではない場合
                    IsEmptyElement = Reader.IsEmptyElement;
                    if (Reader.IsEmptyElement == false)
                    {
                        // ネストする
                        Path.Add(Reader.Name);
                        NestCount++;
                    }

                    // 属性をリストアップ
                    AttrValue = new List<AttrInfo>();
                    while (Reader.MoveToNextAttribute())
                    {
                        AttrInfo attr = new AttrInfo();
                        attr.Name = Reader.Name;
                        attr.Value = Reader.Value;
                        AttrValue.Add(attr);
                    }

                    break;
                case XmlNodeType.EndElement:
                    NodeType = Reader.NodeType;
                    Value = Reader.Name;
                    AttrValue = null;

                    if (Path[Path.Count - 1] == Reader.Name)
                    {
                        // ネストを戻す
                        Path.RemoveAt(Path.Count - 1);
                        NestCount--;
                    }
                    break;
                case XmlNodeType.Text:
                    NodeType = Reader.NodeType;
                    Value = Reader.Value;
                    AttrValue = null;

                    break;
                default:
                    NodeType = XmlNodeType.None;
                    break;
            }

            return true;
        }

        return false;
    }
}

//////////////////////////////////////////////////////////
// GetPath() : 現在のパスを取得する
//////////////////////////////////////////////////////////

public partial class XMLTagReaderClass
{
    public string GetPath()
    {
        string strRet = "";

        if (Path.Count == 0)
        {
            return null;
        }

        foreach (string str in Path)
        {
            strRet += str;
            strRet += System.IO.Path.DirectorySeparatorChar;
        }

        return strRet;
    }
}

//////////////////////////////////////////////////////////
// Close() : 全ての処理を終了する
//////////////////////////////////////////////////////////

public partial class XMLTagReaderClass
{
    public void Close()
    {
        Dispose();
    }
}

//////////////////////////////////////////////////////////
// Dump() : ダンプ処理(デバッグ用)
//////////////////////////////////////////////////////////

public partial class XMLTagReaderClass
{
    public bool Dump()
    {
        if (Reader.ReadState == ReadState.Closed ||
            Reader.ReadState == ReadState.EndOfFile ||
            Reader.ReadState == ReadState.Error)
        {
            return false;
        }

        try
        {
            while (Reader.Read())
            {
                System.Diagnostics.Debug.WriteLine(String.Format("NodeType {0}, Name {1}, Value {2}"
                    , Reader.NodeType.ToString(), Reader.Name.ToString(), Reader.Value.ToString()));

                if (Reader.NodeType == XmlNodeType.Element)
                {
                    // 空の要素かどうかを確認
                    if (Reader.IsEmptyElement)
                    {
                        System.Diagnostics.Debug.WriteLine("   Is Empty Element");
                    }

                    // 属性をリストアップ
                    while (Reader.MoveToNextAttribute())
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("   Attribute {0}, Value {1}"
                            , Reader.Name.ToString(), Reader.Value.ToString()));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("   Exception !!! {0}", ex.Message));
        }

        return true;
    }
}
