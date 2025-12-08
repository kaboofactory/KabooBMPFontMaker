using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public struct XMLPosData
{
    public int id, x, y, width, height, xoffset, yoffset, xadvance, page, count;
    public double u1, v1, u2, v2;
    public string c;
};

namespace BMPFontMaker
{
    class CreateBMPFont
    {
        //////////////////
        /// メンバ変数
        //////////////////

        string mstrMojiset;     // 文字セット
        Bitmap mSource;         // コピー元イメージ
        Graphics mSourceG;      // コピー元イメージ描画用
        FontStyle mFontStyle;   // フォントスタイル

        int mnAscent;           // アセント
        int mnDescent;          // デセント
        int mnEm;               // 1emが何フォントデザイン単位か
        float mfAscentPixel;    // アセント(pixel単位)
        float mfDescentPixel;   // デセント(pixel単位)
        float mfEmSize;         // ピクセル単位のフォントのemサイズ

        List<XMLPosData> listCharsData; // 1文字ごとの座標情報

        //////////////////
        /// ビットマップフォントイメージの作成
        //////////////////

        public bool Create(string strFilename, bool bExport)
        {
            // まず作成するフォントの一覧を作成する
            MakeCharset();

            // コピー元イメージの作成および、フォントサイズの計算
            if (!Prepare())
            {
                return false;
            }

            // イメージの生成
            Generate(strFilename, bExport);

            // ガーベジコレクトを促す
            GC.Collect();

            // 終了
            return true;
        }

        //////////////////
        /// フォント一覧の作成
        //////////////////

        private void MakeCharset()
        {
            // フォントの一覧を覚醒する
            mstrMojiset = "";

            if (Data.HankakuKigou)
            {
                // 半角記号
                mstrMojiset += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~｡｢｣､ ";
            }
            if (Data.HankakuNumber)
            {
                // 半角数字
                mstrMojiset += "0123456789";
            }
            if (Data.HankakuAlphabet)
            {
                // 半角英字
                mstrMojiset += "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            }
            if (Data.HankakuKatakana)
            {
                // 半角カタカナ
                mstrMojiset += "ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ";
            }
            if (Data.ZenkakuKigou)
            {
                // 全角記号
                mstrMojiset += "　、。，．・：；？！゛゜´｀¨＾￣＿ヽヾゝゞ〃仝々ー〇ー―‐／＼～∥｜…‥‘’“”（）〔〕［］｛｝";
                mstrMojiset += "〈〉《》「」『』【】＋－±×÷＝≠＜＞≦≧∞∴♂♀°′″℃￥＄￠￡％＃＆＊＠§☆★○●◎◇◆";
                mstrMojiset += "□■△▲▽▼※〒→←↑↓〓∈∋⊆⊇⊂⊃∪∩∧∨￢⇒⇔∀∃∠⊥⌒∂∇≡≒≪≫√∽∝∫∬Å‰♯♭♪†‡¶◯";
            }
            if (Data.ZenkakuNumber)
            {
                // 全角数字
                mstrMojiset += "０１２３３４５６７８９";
            }
            if (Data.ZenkakuAlphabet)
            {
                // 全角英字
                mstrMojiset += "ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ";
            }
            if (Data.ZenkakuHiragana)
            {
                // 全角ひらがな
                mstrMojiset += "ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとど";
                mstrMojiset += "なにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをん";
            }
            if (Data.ZenkakuKatakana)
            {
                // 全角カタカナ
                mstrMojiset += "ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトド";
                mstrMojiset += "ナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶ";
            }
            if (Data.ZenkakuRussian)
            {
                // 全角ロシア文字
                mstrMojiset += "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            }
            if (Data.ZenkakuLine)
            {
                // 全角罫線
                mstrMojiset += "─│┌┐┘└├┬┤┴┼━┃┏┓┛┗┣┳┫┻╋┠┯┨┷┿┝┰┥┸╂";
            }
            if (Data.ZenkakuOthers)
            {
                // 全角その他
                mstrMojiset += "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζηθικλμνξοπρστυφχψω";
                mstrMojiset += "①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ㍉㌔㌢㍍㌘㌧㌃㌶㍑㍗㌍㌦㌣㌫㍊㌻";
                mstrMojiset += "㎜㎝㎞㎎㎏㏄㎡㍻〝〟№㏍℡㊤㊥㊦㊧㊨㈱㈲㈹㍾㍽≒≡∫∮∑√⊥∠∟⊿∵∩∪";
            }
            if (Data.KanjiExists)
            {
                // 漢字あり
                if (Data.KanjiElementary1)
                {
                    // 小１漢字
                    mstrMojiset += "一右雨円王音下火花貝学気九休玉金空月犬見五口校左三山子四糸字耳七車手十出女小上森人水正生青夕石赤千川先早草足村大男竹中虫町天田土二日入年白八百文木本名目立力林六";
                }
                if (Data.KanjiElementary2)
                {
                    // 小２漢字
                    mstrMojiset += "引羽雲園遠何科夏家歌画回会海絵外角楽活間丸岩顔汽記帰弓牛魚京強教近兄形計元言原戸古午後語工公広交光考行高黄合谷国黒今才細作算止市矢姉思紙寺自時室社弱首秋週春書少";
                    mstrMojiset += "場色食心新親図数西声星晴切雪船線前組走多太体台地池知茶昼長鳥朝直通弟店点電刀冬当東答頭同道読内南肉馬売買麦半番父風分聞米歩母方北毎妹万明鳴毛門夜野友用曜来里理話";
                }
                if (Data.KanjiElementary3)
                {
                    // 小３漢字
                    mstrMojiset += "悪安暗医委意育員院飲運泳駅央横屋温化荷界開階寒感漢館岸起期客究急級宮球去橋業曲局銀区苦具君係軽血決研県庫湖向幸港号根祭皿仕死使始指歯詩次事持式実写者主守取酒受州";
                    mstrMojiset += "拾終習集住重宿所暑助昭消商章勝乗植申身神真深進世整昔全相送想息速族他打対待代第題炭短談着注柱丁帳調追定庭笛鉄転都度投豆島湯登等動童農波配倍箱畑発反坂板皮悲美鼻筆";
                    mstrMojiset += "氷表秒病品負部服福物平返勉放味命面問役薬由油有遊予羊洋葉陽様落流旅両緑礼列練路和";
                }
                if (Data.KanjiElementary4)
                {
                    // 小４漢字
                    mstrMojiset += "愛案以衣位囲胃印英栄塩億加果貨課芽改械害街各覚完官管関観願希季紀喜旗器機議求泣救給挙漁共協鏡競極訓軍郡径型景芸欠結建健験固功好候航康告差菜最材昨札刷殺察参産散残";
                    mstrMojiset += "士氏史司試児治辞失借種周祝順初松笑唱焼象照賞臣信成省清静席積折節説浅戦選然争倉巣束側続卒孫帯隊達単置仲貯兆腸低底停的典伝徒努灯堂働特得毒熱念敗梅博飯飛費必票標不";
                    mstrMojiset += "夫付府副粉兵別辺変便包法望牧末満未脈民無約勇要養浴利陸良料量輪類令冷例歴連老労録";
                }
                if (Data.KanjiElementary5)
                {
                    // 小５漢字
                    mstrMojiset += "圧移因永営衛易益液演応往桜恩可仮価河過賀快解格確額刊幹慣眼基寄規技義逆久旧居許境均禁句群経潔件券険検限現減故個護効厚耕鉱構興講混査再災妻採際在財罪雑酸賛支志枝師";
                    mstrMojiset += "資飼示似識質舎謝授修述術準序招承証条状常情織職制性政勢精製税責績接設舌絶銭祖素総造像増則測属率損退貸態団断築張提程適敵統銅導徳独任燃能破犯判版比肥非備俵評貧布婦";
                    mstrMojiset += "富武復複仏編弁保墓報豊防貿暴務夢迷綿輸余預容略留領";
                }
                if (Data.KanjiElementary6)
                {
                    // 小６漢字
                    mstrMojiset += "異遺域宇映延沿我灰拡革閣割株干巻看簡危机揮貴疑吸供胸郷勤筋系敬警劇激穴絹権憲源厳己呼誤后孝皇紅降鋼刻穀骨困砂座済裁策冊蚕至私姿視詞誌磁射捨尺若樹収宗就衆従縦縮熟";
                    mstrMojiset += "純処署諸除将傷障城蒸針仁垂推寸盛聖誠宣専泉洗染善奏窓創装層操蔵臓存尊宅担探誕段暖値宙忠著庁頂潮賃痛展討党糖届難乳認納脳派拝背肺俳班晩否批秘腹奮並陛閉片補暮宝訪亡";
                    mstrMojiset += "忘棒枚幕密盟模訳郵優幼欲翌乱卵覧裏律臨朗論";
                }
                if (Data.KanjiMiddle)
                {
                    // 中学漢字
                    mstrMojiset += "亜哀挨曖握扱宛嵐依威為畏尉萎偉椅彙違維慰緯壱逸茨芋咽姻淫陰隠韻唄鬱畝浦詠影鋭疫悦越謁閲炎怨宴媛援煙猿";
                    mstrMojiset += "鉛縁艶汚凹押旺欧殴翁奥岡憶臆虞乙俺卸穏佳苛架華菓渦嫁暇禍靴寡箇稼蚊牙瓦雅餓介戒怪拐悔皆塊楷潰壊懐諧劾";
                    mstrMojiset += "崖涯慨蓋該概骸垣柿核殻郭較隔獲嚇穫岳顎掛潟括喝渇葛滑褐轄且釜鎌刈甘汗缶肝冠陥乾勘患貫喚堪換敢棺款閑勧";
                    mstrMojiset += "寛歓監緩憾還環韓艦鑑含玩頑企伎岐忌奇祈軌既飢鬼亀幾棋棄毀畿輝騎宜偽欺儀戯擬犠菊吉喫詰却脚虐及丘朽臼糾";
                    mstrMojiset += "嗅窮巨拒拠虚距御凶叫狂享況峡挟狭恐恭脅矯響驚仰暁凝巾斤菌琴僅緊錦謹襟吟駆惧愚偶遇隅串屈掘窟熊繰勲薫刑";
                    mstrMojiset += "茎契恵啓掲渓蛍傾携継詣慶憬稽憩鶏迎鯨隙撃桁傑肩倹兼剣拳軒圏堅嫌献遣賢謙鍵繭顕懸幻玄弦舷股虎孤弧枯雇誇";
                    mstrMojiset += "鼓錮顧互呉娯悟碁勾孔巧甲江坑抗攻更拘肯侯恒洪荒郊香貢控梗喉慌硬絞項溝綱酵稿衡購乞拷剛傲豪克酷獄駒込頃";
                    mstrMojiset += "昆恨婚痕紺魂墾懇佐沙唆詐鎖挫采砕宰栽彩斎債催塞歳載埼剤崎削柵索酢搾錯咲刹拶撮擦桟惨傘斬暫旨伺刺祉肢施";
                    mstrMojiset += "恣脂紫嗣雌摯賜諮侍滋慈餌璽鹿軸𠮟疾執湿嫉漆芝赦斜煮遮邪蛇酌釈爵寂朱狩殊珠腫趣寿呪需儒囚舟秀臭袖羞愁酬";
                    mstrMojiset += "醜蹴襲汁充柔渋銃獣叔淑粛塾俊瞬旬巡盾准殉循潤遵庶緒如叙徐升召匠床抄肖尚昇沼宵症祥称渉紹訟掌晶焦硝粧詔";
                    mstrMojiset += "奨詳彰憧衝償礁鐘丈冗浄剰畳縄壌嬢錠譲醸拭殖飾触嘱辱尻伸芯辛侵津唇娠振浸紳診寝慎審震薪刃尽迅甚陣尋腎須";
                    mstrMojiset += "吹炊帥粋衰酔遂睡穂随髄枢崇据杉裾瀬是井姓征斉牲凄逝婿誓請醒斥析脊隻惜戚跡籍拙窃摂仙占扇栓旋煎羨腺詮践";
                    mstrMojiset += "箋潜遷薦繊鮮禅漸膳繕狙阻租措粗疎訴塑遡礎双壮荘捜挿桑掃曹曽爽喪痩葬僧遭槽踪燥霜騒藻憎贈即促捉俗賊遜汰";
                    mstrMojiset += "妥唾堕惰駄耐怠胎泰堆袋逮替滞戴滝択沢卓拓託濯諾濁但脱奪棚誰丹旦胆淡嘆端綻鍛弾壇恥致遅痴稚緻畜逐蓄秩窒";
                    mstrMojiset += "嫡沖抽衷酎鋳駐弔挑彫眺釣貼超跳徴嘲澄聴懲勅捗沈珍朕陳鎮椎墜塚漬坪爪鶴呈廷抵邸亭貞帝訂逓偵堤艇締諦泥摘";
                    mstrMojiset += "滴溺迭哲徹撤添塡殿斗吐妬途渡塗賭奴怒到逃倒凍唐桃透悼盗陶塔搭棟痘筒稲踏謄藤闘騰洞胴瞳峠匿督篤栃凸突屯";
                    mstrMojiset += "豚頓貪鈍曇丼那奈梨謎鍋軟尼弐匂虹尿妊忍寧捻粘悩濃把覇婆罵杯排廃輩培陪媒賠伯拍泊迫剝舶薄漠縛爆箸肌鉢髪";
                    mstrMojiset += "伐抜罰閥氾帆汎伴阪畔般販斑搬煩頒範繁藩蛮盤妃彼披卑疲被扉碑罷避尾眉微膝肘匹泌姫漂苗描猫浜賓頻敏瓶扶怖";
                    mstrMojiset += "阜附訃赴浮符普腐敷膚賦譜侮舞封伏幅覆払沸紛雰噴墳憤丙併柄塀幣弊蔽餅壁璧癖蔑偏遍哺捕舗募慕簿芳邦奉抱泡";
                    mstrMojiset += "胞俸倣峰砲崩蜂飽褒縫乏忙坊妨房肪某冒剖紡傍帽貌膨謀頰朴睦僕墨撲没勃堀奔翻凡盆麻摩磨魔昧埋膜枕又抹慢漫";
                    mstrMojiset += "魅岬蜜妙眠矛霧娘冥銘滅免麺茂妄盲耗猛網黙紋冶弥厄躍闇喩愉諭癒唯幽悠湧猶裕雄誘憂融与誉妖庸揚揺溶腰瘍踊";
                    mstrMojiset += "窯擁謡抑沃翼拉裸羅雷頼絡酪辣濫藍欄吏痢履璃離慄柳竜粒隆硫侶虜慮了涼猟陵僚寮療瞭糧厘倫隣瑠涙累塁励戻鈴";
                    mstrMojiset += "零霊隷齢麗暦劣烈裂恋廉錬呂炉賂露弄郎浪廊楼漏籠麓賄脇惑枠湾腕";
                }
                if (Data.KanjiName)
                {
                    // 人名漢字
                    mstrMojiset += "卜乃叉也勺之已巳匁壬尤勿巴丑云允廿卯仔乎戊凧叶只疋汀禾弘亥此弛亘而肋瓜牟汐尖汝圭托丞辻旭匡亙伍庄夷凪伊亦收曳牡辿劫壯宏芹佃佛兎冴佑芭玖灸吾迄";
                    mstrMojiset += "庇沌甫芙迂吻亨伽宋坐吞步杜每辰巫芦杖邑孜伶汲酉灼芥李杏狀昌拂昏帖社坦欣阿爭苔沓忽卑尭陀些茅杷昂亞來宕杭怜穹迪或侑竺於茄苺昊肴庚奄茉朋祁苑斧卷";
                    mstrMojiset += "枇杵拔孟兒其沫侃函珊洲柊哉柘茜茸勁奎昴洸洵珈珀拜恆侮勉祈祉突者俠俐玲亮穿殆祢姪祐盃柏毘姥柾俣籾耶柚宥洛卽巷竿臥彦柑恰娃祇衿按恢廻胤俄郁迦頁珂";
                    mstrMojiset += "胡栗浬桧哩恕耽狹倭乘倖紐峻凌屑哨豹臭隼啄祝悌悔莞晋海凉秦神訊圃赳倦莉氣秤祖祐桂閃峯娩桔紗涉笈烏祕荻栞砥莫紘矩眞峨晒柴栖朔窄峽套桐晟晏俱浩郞畠";
                    mstrMojiset += "砧莊挽挺晃晄狼釘紬雫朗徠祷彪淨祥桶猪釧訣菩敍兜捧畢掬萄袈將專梶從圈琢晝梯彬條寅舵陷淀國捺帶萌彗笠脩晦淋笙菱梁絆淚惇琉淳掠梧渚惟雀梛砦笹萊崚惚";
                    mstrMojiset += "偲晨晚埜梓毬巢庵悉皐這袴捷羚絃菅冨逗牽逞埴敏梅椛逢捲萠眸凰菫菖梢惺隈湊惡凱椀敦卿稀翔琵淵喬琶萱禄斐琥椋筈葡釉瑛裡惠堯雁琳皓黃堰單焚曾堺萬葵遥";
                    mstrMojiset += "董琢猪竣巽虜著視葺萩戟喰腔爲湛惣都粥甥渚粟絢硯厨棲貰疏喧揃閏逸剩搜渴揭斯盜渥湘欽堵焰犀虛脹惹黑寓筑喋智註溫溜詢頌瑚蒐蓉嵯楊椰葦裟蓑塙跨蓮馳稜";
                    mstrMojiset += "蒔煉楢瑶滉幌舜遁嵩楯碗暉煌嘩獅傭碑愼碓蒲禽奧楓圓楕詫廊裝與馴楠祿鳩煤鼎禎椿搖傳蒼溢稔勤蒙牒暑瑞靖碎稟煮楚樺蔭禎斡壽摑鳶寢漱盡團綠蔣窪漢粹齊奬";
                    mstrMojiset += "署嘉僧綸綺實槇榮遙僞嘆寬滯禍榎颯賓福蔓暢魁聡綜漕槍銑碩翠賑槙榛肇蔦輔碧箔厩箕頗緋蓬嶋鞄綴槌裳嘗爾膏閤漣鳳竪榊瑳綾摺慧澁播劍樋蕎徵憎撰樣瘦蕃彈";
                    mstrMojiset += "鞍儉價撒幡緣磐稻增醇練節噌穀糊墨層樟蕉噂髮撞駈廣緖蝶諏德歎賣醉蕨鄭篇誼毅蕪黎蝦劉魯樂駕熙諄凛鋒諒遼嬉槻撫凜廟賴樫醍龍曆衞諸蕾蹄默曉黛燈靜橫薙";
                    mstrMojiset += "險縞橙薗燒燕樽戰錫鴨憐歷謂縣謁蕗錄醐輯勳澪橘叡錆鞘器錘燎鋸窺諺錐鮎徽薰螺駿鍬曙檢繁輿濡篠檜燭嶺謠彌擊霞縱瞥鞠鴻壕穗燦檎藁薩濕擢檀瓢應禪磯鍊戲";
                    mstrMojiset += "鎭鯉穣鞭鎧藏蟬醬藝藥轉燿謹蹟儲麿壘襖叢雜簞櫂鵜雛禮顚難贈櫓櫛簾瀨繫繡禱蘭曝麒寵鯛懷類蟹鵬壞瀕蘇瀧懲獸禰馨騷嚴纂巌孃耀櫻轟纏飜鰯欄蠟鷄攝疊聽鷗";
                    mstrMojiset += "饗鑄讃驍覽臟穰灘響顯巖纖鷲鱒驗鷹麟鱗讓釀鷺廳";
                }
                if (Data.KanjiOther)
                {
                    // その他よく使うっぽい漢字
                    mstrMojiset += "乖倶冤嘘噛叩吊呆埠壷壺牢嬌屁怯憚戌掻掴捏撥扮拗梱榴歪洩涵渠涛濤涜炒瑕玻畴疇癌疵疹眩碍砒竄糞繋繞罠羯聘聾肛胚蛾蛙蠍蝋謳讚諜誹謗躊躇軣醤隕頚頸飴";
                    mstrMojiset += "餃騙鮭鹸麹麸麩鰺鮑鰻鰹鰈鱚鮫鯖鰆鯱鮹鱈鱧鮪鮨鮓韮葱椚欅棗椒楪";
                }
            }
            if (Data.OthersExists)
            {
                // その他の文字

                // 改行コードがあれば取り除く
                string strAdd = Data.OthersList.Replace("\r", "").Replace("\n", "");

                // 追加する
                mstrMojiset += strAdd;
            }

            // 最後に重複を除く
            mstrMojiset = new string(mstrMojiset.ToCharArray().Distinct().ToArray());
        }

        //////////////////
        /// 前処理
        //////////////////

        private bool Prepare()
        {
            int nFontHeight, nImageWidth, nImageHeight;

            // 前処理

            // コピー元イメージを作成する
            mSource = new Bitmap(Data.ImageSize.Width, Data.ImageSize.Height);
            mSourceG = Graphics.FromImage(mSource);

            // アンチエイリアス
            mSourceG.TextRenderingHint = Data.Smooth;

            // アセンド・デセンドサイズを計算する
            mFontStyle = 0;

            if (Data.DrawFont.Bold)
            {
                mFontStyle |= FontStyle.Bold;
            }
            if (Data.DrawFont.Italic)
            {
                mFontStyle |= FontStyle.Italic;
            }
            if (Data.DrawFont.Strikeout)
            {
                mFontStyle |= FontStyle.Strikeout;
            }
            if (Data.DrawFont.Underline)
            {
                mFontStyle |= FontStyle.Underline;
            }
            if (mFontStyle == 0)
            {
                mFontStyle = FontStyle.Regular;
            }

            mnAscent = Data.DrawFont.FontFamily.GetCellAscent(mFontStyle);
            mnDescent = Data.DrawFont.FontFamily.GetCellDescent(mFontStyle);
            mnEm = Data.DrawFont.FontFamily.GetEmHeight(mFontStyle);
            mfAscentPixel = (Data.DrawFont.SizeInPoints * (96.0f / 72.0f)) * ((float)mnAscent / (float)mnEm);
            mfDescentPixel = (Data.DrawFont.SizeInPoints * (96.0f / 72.0f)) * ((float)mnDescent / (float)mnEm);
            mfEmSize = (float)Data.DrawFont.Height * (float)mnEm / (float)Data.DrawFont.FontFamily.GetLineSpacing(mFontStyle);

            nFontHeight = (int)Math.Ceiling(mfAscentPixel + mfDescentPixel);
            nImageWidth = nFontHeight + Data.PaddingTop + Data.PaddingBottom + 2;
            nImageHeight = nFontHeight + Data.PaddingRight + Data.PaddingLeft + 2;

            if ((nImageWidth > Data.ImageSize.Width) || (nImageHeight > Data.ImageSize.Height))
            {
                // イメージが小さすぎる
                MessageBox.Show(String.Format("出力イメージサイズが小さすぎるため、キャンセルします 幅 ({0}) 高さ({1})", nImageWidth, nImageHeight)
                    , "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                return false;
            }

            return true;
        }

        //////////////////
        /// 生成
        //////////////////

        private void Generate(string strFilename, bool bExport)
        {
            int nImageNumber = 0;   // イメージ連番
            int nX = 0, nY = 0;     // 現在の左上座標
            int nMojiCount = 0;     // イメージ内での文字数カウント
            int nHeight, nLeft, nRight, nWidth, nTempX, nTempY;
            int nMeasureWidth;
            SizeF size;
            Bitmap Dest = new Bitmap(Data.ImageSize.Width, Data.ImageSize.Height);
            Graphics DestG = Graphics.FromImage(Dest);
            Brush FontBrush = new SolidBrush(Data.DrawFontColor);
            Pen LinePen = new Pen(Data.LineColor);
            Pen EdgePen = new Pen(Data.EdgeColor, Data.EdgePenWidth);
            System.Drawing.Drawing2D.GraphicsPath Path = new System.Drawing.Drawing2D.GraphicsPath();

            XMLPosData posAdd;
            XMLTagWriterClass xml = null;
            StreamWriter stream = null;

            TextureBrush FontTextureBrush = null;
            Pen EdgeTexturePen = null;

            LinearGradientBrush FontGradientBrush = null;
            Pen EdgeGradientPen = null;

            // 高さを指定する
            if (Data.Edge)
            {
                nHeight = (int)Math.Ceiling(mfAscentPixel + mfDescentPixel + (float)Data.EdgePenWidth);
            }
            else
            {
                nHeight = (int)Math.Ceiling(mfAscentPixel + mfDescentPixel);
            }

            // フォントにテクスチャブラシを使用する
            switch (Data.DrawFontColorType)
            {
                case EnumBrushType.Image:
                    if (Data.DrawFontColorImageScaling)
                    {
                        // イメージを拡大縮小する
                        double dRate = (double)nHeight / (double)Data.DrawFontColorImage.Height;
                        Bitmap bmpTemp = new Bitmap((int)Math.Ceiling((double)Data.DrawFontColorImage.Width * dRate), nHeight);
                        Graphics g = Graphics.FromImage(bmpTemp);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(Data.DrawFontColorImage, 0, 0, bmpTemp.Width, bmpTemp.Height);
                        g.Dispose();
                        FontTextureBrush = new TextureBrush(bmpTemp, WrapMode.TileFlipXY);
                    }
                    else
                    {
                        // イメージを拡大縮小しない
                        FontTextureBrush = new TextureBrush(Data.DrawFontColorImage, WrapMode.TileFlipXY);
                    }
                    break;
                case EnumBrushType.Gradient:
                    FontGradientBrush = GetFontGradientBrush();
                    break;
            }

            // 縁取りにテクスチャブラシを使用する
            switch(Data.EdgeColorType)
            {
                case EnumBrushType.Image:
                    if (Data.EdgeColorImageScaling)
                    {
                        // イメージを拡大縮小する
                        double dRate = (double)nHeight / (double)Data.EdgeColorImage.Height;
                        Bitmap bmpTemp = new Bitmap((int)Math.Ceiling((double)Data.EdgeColorImage.Width * dRate), nHeight);
                        Graphics g = Graphics.FromImage(bmpTemp);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(Data.EdgeColorImage, 0, 0, bmpTemp.Width, bmpTemp.Height);
                        g.Dispose();
                        EdgeTexturePen = new Pen(new TextureBrush(bmpTemp, WrapMode.TileFlipXY), Data.EdgePenWidth);
                    }
                    else
                    {
                        // イメージを拡大縮小しない
                        EdgeTexturePen = new Pen(new TextureBrush(Data.EdgeColorImage, WrapMode.TileFlipXY), Data.EdgePenWidth);
                    }
                    break;
                case EnumBrushType.Gradient:
                    EdgeGradientPen = new Pen(GetEdgeGradientBrush(), Data.EdgePenWidth);
                    break;
            }

            // ビットマップリストの初期化
            Data.listBitmap = new List<Bitmap>();

            // 座標情報の初期化
            listCharsData = new List<XMLPosData>();

            // コピー先イメージの初期化
            DestG.Clear(Data.BgColor);

            for (int nPos = 0; nPos < mstrMojiset.Length; nPos++)
            {
                // バックグラウンドビットマップをクリアする
                mSourceG.Clear(Data.BgColor);

                // 文字を描画する
                if (Data.Edge)
                {
                    ///////////////
                    // 縁取りあり
                    ///////////////

                    // 初期化
                    if (Data.EdgeAntialias)
                    {
                        // アンチエイリアスあり
                        mSourceG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    }
                    else
                    {
                        // アンチエイリアスなし
                        mSourceG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                    }
                    mSourceG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    // パスを生成
                    Path.Reset();
                    Path.AddString(mstrMojiset[nPos].ToString(), Data.DrawFont.FontFamily, (int)mFontStyle, mfEmSize, new PointF(0.0f, Data.EdgePenWidth * 0.5f), StringFormat.GenericDefault);

                    if (Data.EdgeFirst)
                    {
                        // 縁を描画
                        switch(Data.EdgeColorType)
                        {
                            case EnumBrushType.Solid:
                                mSourceG.DrawPath(EdgePen, Path);
                                break;
                            case EnumBrushType.Image:
                                mSourceG.DrawPath(EdgeTexturePen, Path);
                                break;
                            case EnumBrushType.Gradient:
                                mSourceG.DrawPath(EdgeGradientPen, Path);
                                break;
                        }
                        
                        // 中を描画
                        if (!Data.EdgeOnly)
                        {
                            switch (Data.DrawFontColorType)
                            {
                                case EnumBrushType.Solid:
                                    mSourceG.FillPath(FontBrush, Path);
                                    break;
                                case EnumBrushType.Image:
                                    mSourceG.FillPath(FontTextureBrush, Path);
                                    break;
                                case EnumBrushType.Gradient:
                                    mSourceG.FillPath(FontGradientBrush, Path);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // 中を描画
                        if (!Data.EdgeOnly)
                        {
                            switch (Data.DrawFontColorType)
                            {
                                case EnumBrushType.Solid:
                                    mSourceG.FillPath(FontBrush, Path);
                                    break;
                                case EnumBrushType.Image:
                                    mSourceG.FillPath(FontTextureBrush, Path);
                                    break;
                                case EnumBrushType.Gradient:
                                    mSourceG.FillPath(FontGradientBrush, Path);
                                    break;
                            }
                        }
                        // 縁を描画
                        switch (Data.EdgeColorType)
                        {
                            case EnumBrushType.Solid:
                                mSourceG.DrawPath(EdgePen, Path);
                                break;
                            case EnumBrushType.Image:
                                mSourceG.DrawPath(EdgeTexturePen, Path);
                                break;
                            case EnumBrushType.Gradient:
                                mSourceG.DrawPath(EdgeGradientPen, Path);
                                break;
                        }
                    }

                    // 描画幅を計算
                    size = mSourceG.MeasureString(mstrMojiset[nPos].ToString(), Data.DrawFont, Data.ImageSize.Width);
                    nMeasureWidth = (int)Math.Ceiling(size.Width + Data.EdgePenWidth);
                }
                else
                {
                    ///////////////
                    // 縁取りなし
                    ///////////////

                    // 描画
                    switch (Data.DrawFontColorType)
                    {
                        case EnumBrushType.Solid:
                            mSourceG.DrawString(mstrMojiset[nPos].ToString(), Data.DrawFont, FontBrush, 0.0f, 0.0f);
                            break;
                        case EnumBrushType.Image:
                            mSourceG.DrawString(mstrMojiset[nPos].ToString(), Data.DrawFont, FontTextureBrush, 0.0f, 0.0f);
                            break;
                        case EnumBrushType.Gradient:
                            mSourceG.DrawString(mstrMojiset[nPos].ToString(), Data.DrawFont, FontGradientBrush, 0.0f, 0.0f);
                            break;
                    }

                    // 描画幅を計算
                    size = mSourceG.MeasureString(mstrMojiset[nPos].ToString(), Data.DrawFont, Data.ImageSize.Width);
                    nMeasureWidth = (int)Math.Ceiling(size.Width);
                }

                // 左端の座標を取得する
                nLeft = -1;
                for (int x = 0; x < nMeasureWidth; x++)
                {
                    for (int y = 0; y < nHeight; y++)
                    {
                        if (mSource.GetPixel(x, y).ToArgb() != Data.BgColor.ToArgb())
                        {
                            nLeft = x;
                            break;
                        }
                    }
                    if (nLeft >= 0)
                    {
                        break;
                    }
                }

                // 右端の座標を取得する
                nRight = nMeasureWidth;
                for (int x = nMeasureWidth - 1; x >= 0; x--)
                {
                    for (int y = 0; y < nHeight; y++)
                    {
                        if (mSource.GetPixel(x, y).ToArgb() != Data.BgColor.ToArgb())
                        {
                            nRight = x;
                            break;
                        }
                    }
                    if (nRight < nMeasureWidth)
                    {
                        break;
                    }
                }

                // 不検出時判定
                if (nLeft == -1 && nRight == nMeasureWidth)
                {
                    nLeft++;
                    nRight--;
                }

                // この時点で範囲内ピクセルの左限がnLeft、右限がnRight、上限が0、下限がnHeight - 1
                nWidth = (nRight + 1) - nLeft;

                // 現在位置にフォントを置けるかチェックする
                if (nX + nWidth + 2 + Data.PaddingLeft + Data.PaddingRight >= Data.ImageSize.Width) // 2は罫線分
                {
                    // 改行
                    nX = 0;
                    nY += nHeight + 2 + Data.PaddingTop + Data.PaddingBottom;

                    // 改行後にイメージ範囲外になるとチェックする
                    if (nY + nHeight + 3 + Data.PaddingTop + Data.PaddingBottom >= Data.ImageSize.Height) // 2は罫線分
                    {
                        // 次のイメージへ
                        Data.listBitmap.Add(Dest);
                        Dest = new Bitmap(Data.ImageSize.Width, Data.ImageSize.Height);
                        DestG = Graphics.FromImage(Dest);
                        DestG.Clear(Data.BgColor);
                        nMojiCount = 0;
                        nX = 0;
                        nY = 0;
                    }
                }

                // 罫線を描画する
                nTempX = nX + nWidth + 1 + Data.PaddingLeft + Data.PaddingRight;
                nTempY = nY + nHeight + 1 + Data.PaddingTop + Data.PaddingBottom;
                DestG.DrawLine(LinePen, nX, nY, nTempX, nY);
                DestG.DrawLine(LinePen, nX, nY, nX, nTempY);
                DestG.DrawLine(LinePen, nTempX, nY, nTempX, nTempY);
                DestG.DrawLine(LinePen, nX, nTempY, nTempX, nTempY);

                // イメージをコピーする
                DestG.DrawImage(mSource, nX + 1 + Data.PaddingLeft, nY + 1 + Data.PaddingTop
                    , new Rectangle(nLeft, 0, nWidth, nHeight), GraphicsUnit.Pixel);

                // 文字情報を保持する
                posAdd.id = Convert.ToInt32(mstrMojiset[nPos]);
                posAdd.x = nX + 1;
                posAdd.y = nY + 1;
                posAdd.width = nWidth + Data.PaddingLeft + Data.PaddingRight;
                posAdd.height = nHeight + Data.PaddingTop + Data.PaddingBottom;
                if (Data.XMLFixWidthEnable)
                {
                    posAdd.xoffset = (int)Math.Ceiling((double)(Data.XMLFixWidth - posAdd.width) * 0.5);
                    posAdd.xadvance = Data.XMLFixWidth;
                }
                else
                {
                    posAdd.xoffset = 0;
                    posAdd.xadvance = posAdd.width;
                }
                posAdd.yoffset = 0;
                posAdd.page = Data.listBitmap.Count;
                posAdd.u1 = (double)posAdd.x / (double)Data.ImageSize.Width;
                posAdd.v1 = (double)posAdd.y / (double)Data.ImageSize.Height;
                posAdd.u2 = (double)(posAdd.x + posAdd.width) / (double)Data.ImageSize.Width;
                posAdd.v2 = (double)(posAdd.y + posAdd.height) / (double)Data.ImageSize.Height;
                if (Data.XMLUReverse)
                {
                    posAdd.u1 = 1.0 - posAdd.u1;
                    posAdd.u2 = 1.0 - posAdd.u2;
                }
                if (Data.XMLVReverse)
                {
                    posAdd.v1 = 1.0 - posAdd.v1;
                    posAdd.v2 = 1.0 - posAdd.v2;
                }
                posAdd.c = mstrMojiset[nPos].ToString();
                posAdd.count = nPos;
                listCharsData.Add(posAdd);

                // 位置を移動する
                nX += nWidth + 2 + Data.PaddingLeft + Data.PaddingRight;
                nMojiCount++;
            }

            // ビットマップリストに追加する
            Data.listBitmap.Add(Dest);

            // ビットマップリストを出力する
            if (bExport)
            {
                /////////////////////
                // イメージの出力
                /////////////////////

                nImageNumber = 0;
                foreach (Bitmap bmp in Data.listBitmap)
                {
                    bool bSave;
                    string Filename;

                    if (Data.SaveImageFormat == EnumImageFormat.Png)
                    {
                        Filename = String.Format("{0}_{1}.png", strFilename, nImageNumber);
                    }
                    else
                    {
                        Filename = String.Format("{0}_{1}.bmp", strFilename, nImageNumber);
                    }

                    nImageNumber++;
                    bSave = true;

                    if (System.IO.File.Exists(Filename))
                    {
                        DialogResult ret = MessageBox.Show(String.Format("{0} は既に存在します。上書きしますか？", Filename), "警告", MessageBoxButtons.YesNo);

                        if (ret == DialogResult.No)
                        {
                            // 上書きしない
                            bSave = false;
                        }
                    }
                    if (bSave)
                    {
                        if (Data.SaveImageFormat == EnumImageFormat.Png)
                        {
                            bmp.Save(Filename, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            bmp.Save(Filename, System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                    }
                }

                /////////////////////
                // XMLの出力
                /////////////////////

                // XMLファイル名を指定
                xml = new XMLTagWriterClass(String.Format("{0}.xml", strFilename));
                stream = new StreamWriter(String.Format("{0}.fnt", strFilename), false, Encoding.UTF8);

                // fontセクションの開始
                xml.BeginElement("font");

                // info(XML,Text)
                xml.BeginElement("info");
                xml.EndElement();
                stream.WriteLine("info");

                // common(XML)
                xml.BeginElement("common");
                xml.WriteAttribute("lineHeight", nHeight.ToString());
                xml.WriteAttribute("base", (nHeight + Data.PaddingTop - (int)mfDescentPixel).ToString());
                xml.WriteAttribute("scaleW", Data.ImageSize.Width.ToString());
                xml.WriteAttribute("scaleH", Data.ImageSize.Height.ToString());
                xml.WriteAttribute("pages", Data.listBitmap.Count.ToString());
                xml.EndElement();

                // common(Text)
                stream.WriteLine(String.Format("common lineHeight={0} base={1} scaleW={2} scaleH={3} pages={4}"
                    , nHeight
                    , (nHeight + Data.PaddingTop - (int)mfDescentPixel)
                    , Data.ImageSize.Width
                    , Data.ImageSize.Height
                    , Data.listBitmap.Count));

                // pages(XML,Text)
                xml.BeginElement("pages");
                nImageNumber = 0;
                foreach (Bitmap bmp in Data.listBitmap)
                {
                    string Filename;

                    if (Data.SaveImageFormat == EnumImageFormat.Png)
                    {
                        Filename = String.Format("{0}_{1}.png", strFilename, nImageNumber);
                    }
                    else
                    {
                        Filename = String.Format("{0}_{1}.bmp", strFilename, nImageNumber);
                    }

                    // page(XML)
                    xml.BeginElement("page");
                    xml.WriteAttribute("id", nImageNumber.ToString());
                    xml.WriteAttribute("file", System.IO.Path.GetFileName(Filename));
                    xml.EndElement();

                    // page(Text)
                    stream.WriteLine(String.Format("page id={0} file=\"{1}\"", nImageNumber, System.IO.Path.GetFileName(Filename)));

                    nImageNumber++;
                }
                xml.EndElement();

                // chars(XML)
                xml.BeginElement("chars");
                xml.WriteAttribute("count", listCharsData.Count.ToString());

                // chars(Text)
                stream.WriteLine(String.Format("chars count={0}", listCharsData.Count));

                // char(XML)
                foreach (XMLPosData dat in listCharsData)
                {
                    xml.BeginElement("char");
                    xml.WriteAttribute("id", dat.id.ToString());
                    xml.WriteAttribute("x", dat.x.ToString());
                    xml.WriteAttribute("y", dat.y.ToString());
                    xml.WriteAttribute("width", dat.width.ToString());
                    xml.WriteAttribute("height", dat.height.ToString());
                    xml.WriteAttribute("xoffset", dat.xoffset.ToString());
                    xml.WriteAttribute("yoffset", dat.yoffset.ToString());
                    xml.WriteAttribute("xadvance", dat.xadvance.ToString());
                    xml.WriteAttribute("page", dat.page.ToString());
                    xml.WriteAttribute("chnl", "15");
                    if (Data.XMLEx)
                    {
                        // 独自拡張情報
                        xml.WriteAttribute("u1", String.Format("{0:F8}", dat.u1));
                        xml.WriteAttribute("v1", String.Format("{0:F8}", dat.v1));
                        xml.WriteAttribute("u2", String.Format("{0:F8}", dat.u2));
                        xml.WriteAttribute("v2", String.Format("{0:F8}", dat.v2));
                        xml.WriteAttribute("count", dat.count.ToString());
                        xml.WriteAttribute("c", dat.c.ToString());
                    }
                    xml.EndElement();
                }
                xml.EndElement(); // </chars>
                xml.EndElement(); // </font>
                xml.Close();
                xml.Dispose();

                // char(Text)
                foreach (XMLPosData dat in listCharsData)
                {
                    stream.Write(String.Format("char id={0} x={1} y={2} width={3} height={4} xoffset={5} yoffset={6} xadvance={7} page={8} chnl=15"
                        , dat.id
                        , dat.x
                        , dat.y
                        , dat.width
                        , dat.height
                        , dat.xoffset
                        , dat.yoffset
                        , dat.xadvance
                        , dat.page));
                    if (Data.XMLEx)
                    {
                        stream.Write(String.Format(" u1={0:F8} v1={1:F8} u2={2:F8} v2={3:F8} count={4} c={5}"
                            , dat.u1
                            , dat.v1
                            , dat.u2
                            , dat.v2
                            , dat.count
                            , dat.c));
                    }
                    stream.WriteLine();
                }
                stream.Close();
                stream.Dispose();

                // 完了報告
                MessageBox.Show(String.Format("{0}枚のイメージ出力を完了しました", Data.listBitmap.Count), "完了", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
        }

        ////////////////////////////////
        /// グラデーションブラシの生成
        ////////////////////////////////

        LinearGradientBrush GetFontGradientBrush()
        {
            // 現在の設定を元にブラシを作成する
            LinearGradientBrush retBrush;
            ColorBlend brend = new ColorBlend();
            int nCount = 0;

            for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
            {
                if ((n == 0) || (n == SetLinearGradientBrushForm.BrushDataMaxCount - 1) || (Data.FontGradientData[n].bEnable))
                {
                    nCount++;
                }
            }

            Color[] colors = new Color[nCount];
            float[] positions = new float[nCount];

            nCount = 0;
            for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
            {
                if ((n == 0) || (n == SetLinearGradientBrushForm.BrushDataMaxCount - 1) || (Data.FontGradientData[n].bEnable))
                {
                    colors[nCount] = Data.FontGradientData[n].GradientColor;
                    if (n == 0)
                    {
                        positions[nCount] = 0.0f;
                    }
                    else if (n == SetLinearGradientBrushForm.BrushDataMaxCount - 1)
                    {
                        positions[nCount] = 1.0f;
                    }
                    else
                    {
                        positions[nCount] = (float)Data.FontGradientData[n].nPosition * 0.01f;
                    }
                    nCount++;
                }
            }

            brend.Colors = colors;
            brend.Positions = positions;

            retBrush = new LinearGradientBrush(
                new PointF((float)Data.FontGradientStartX * 0.01f * (float)Data.FontGradientWidth, (float)Data.FontGradientStartY * 0.01f * (float)Data.FontGradientHeight)
                , new PointF((float)Data.FontGradientEndX * 0.01f * (float)Data.FontGradientWidth, (float)Data.FontGradientEndY * 0.01f * (float)Data.FontGradientHeight)
                , Data.FontGradientData[0].GradientColor
                , Data.FontGradientData[SetLinearGradientBrushForm.BrushDataMaxCount - 1].GradientColor);
            retBrush.WrapMode = Data.FontGradientWrapMode;
            retBrush.InterpolationColors = brend;
            retBrush.RotateTransform((float)Data.FontGradientAngle);

            return retBrush;
        }

        LinearGradientBrush GetEdgeGradientBrush()
        {
            // 現在の設定を元にブラシを作成する
            LinearGradientBrush retBrush;
            ColorBlend brend = new ColorBlend();
            int nCount = 0;

            for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
            {
                if ((n == 0) || (n == SetLinearGradientBrushForm.BrushDataMaxCount - 1) || (Data.EdgeGradientData[n].bEnable))
                {
                    nCount++;
                }
            }

            Color[] colors = new Color[nCount];
            float[] positions = new float[nCount];

            nCount = 0;
            for (int n = 0; n < SetLinearGradientBrushForm.BrushDataMaxCount; n++)
            {
                if ((n == 0) || (n == SetLinearGradientBrushForm.BrushDataMaxCount - 1) || (Data.EdgeGradientData[n].bEnable))
                {
                    colors[nCount] = Data.EdgeGradientData[n].GradientColor;
                    if (n == 0)
                    {
                        positions[nCount] = 0.0f;
                    }
                    else if (n == SetLinearGradientBrushForm.BrushDataMaxCount - 1)
                    {
                        positions[nCount] = 1.0f;
                    }
                    else
                    {
                        positions[nCount] = (float)Data.EdgeGradientData[n].nPosition * 0.01f;
                    }
                    nCount++;
                }
            }

            brend.Colors = colors;
            brend.Positions = positions;

            retBrush = new LinearGradientBrush(
                new PointF((float)Data.EdgeGradientStartX * 0.01f * (float)Data.EdgeGradientWidth, (float)Data.EdgeGradientStartY * 0.01f * (float)Data.EdgeGradientHeight)
                , new PointF((float)Data.EdgeGradientEndX * 0.01f * (float)Data.EdgeGradientWidth, (float)Data.EdgeGradientEndY * 0.01f * (float)Data.EdgeGradientHeight)
                , Data.EdgeGradientData[0].GradientColor
                , Data.EdgeGradientData[SetLinearGradientBrushForm.BrushDataMaxCount - 1].GradientColor);
            retBrush.WrapMode = Data.EdgeGradientWrapMode;
            retBrush.InterpolationColors = brend;
            retBrush.RotateTransform((float)Data.EdgeGradientAngle);

            return retBrush;
        }

    }
}
