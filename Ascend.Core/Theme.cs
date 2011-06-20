using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Theme : Entity
    {
        public string Name { get; set; }
        public string Stylesheet { get; set; }
        public string Banner { get; set; }

        public string[] LoginBanners { get; set; }
        public string[] LoginInfos { get; set; }

        public string CustomLogin { get; set; }
        public string CustomSite { get; set; }
        
        public IDictionary<string, string> Colors { get; set; }
        public IDictionary<string, ThemeFont> Fonts { get; set; }
        public IDictionary<string, ThemeBox> Boxes { get; set; }

        public string ResolveColor(string c)
        {
            return _ResolveColor(c, 0);
        }

        string _ResolveColor(string c, int depth)
        {
            if (depth > 10)
            {
                return "#f00";
            }
            if (null == Colors ||
                String.IsNullOrEmpty(c) ||
                c[0] == '#' ||
                !Colors.ContainsKey(c)) return c;
            var a = Colors[c];
            if (a.Length > 0 && a[0] != '#')
            {
                return _ResolveColor(a, depth + 1);
            }
            return a;
        }

        public string ResolveFont(string f)
        {
            if (String.IsNullOrEmpty(f) ||
                null == Fonts ||
                !Fonts.ContainsKey(f))
            {
                return String.Empty;
            }
            return Fonts[f].ToString(this);
        }

        public string ResolveBox(string b)
        {
            if (String.IsNullOrEmpty(b) ||
                null == Boxes ||
                !Boxes.ContainsKey(b))
            {
                return String.Empty;
            }
            return Boxes[b].ToString(this);        
        }
    }

    public class ThemeFont
    {
        public string Color { get; set; }
        public string Family { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public bool Underline { get; set; }
        public bool Italic { get; set; }
        public bool Caps { get; set; }

        public string ToString(Theme t)
        {
            var a = new StylesheetStringBuilder();
            a.AppendAttribute("color", t.ResolveColor(Color));
            a.AppendAttribute("font-family", Family);
            a.AppendAttribute("font-size", Size);
            a.AppendAttribute("font-weight", Weight);
            if (Underline) a.AppendAttribute("text-decoration", "underline");
            if (Italic) a.AppendAttribute("font-style", "italic");
            if (Caps) a.AppendAttribute("text-transform", "uppercase");
            return a.ToString();
        }
    }

    public class ThemeBox
    {
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }

        public string Width { get; set; }
        public string Height { get; set; }
        public ThemeSpacing Margin { get; set; }
        public ThemeSpacing Padding { get; set; }

        public ThemeBorder BorderTop { get; set; }
        public ThemeBorder BorderBottom { get; set; }
        public ThemeBorder BorderLeft { get; set; }
        public ThemeBorder BorderRight { get; set; }

        public string ToString(Theme t)
        {
            var a = new StylesheetStringBuilder();
            a.AppendAttribute("background-color", t.ResolveColor(BackgroundColor));
            a.AppendAttribute("background-image", BackgroundImage);
            a.AppendAttribute("width", Width);
            a.AppendAttribute("height", Height);
            if (null != Margin && !Margin.IsEmpty) a.AppendAttribute("margin", Margin.ToString());
            if (null != Padding && !Padding.IsEmpty) a.AppendAttribute("padding", Padding.ToString());
            if (ThemeBorder.AreSame(BorderTop, BorderBottom, BorderLeft, BorderRight))
            {
                a.AppendAttribute("border", BorderTop.ToString(t));
            }
            else
            {
                if (null != BorderTop && !BorderTop.IsEmpty) a.AppendAttribute("border-top", BorderTop.ToString(t));
                if (null != BorderBottom && !BorderBottom.IsEmpty) a.AppendAttribute("border-bottom", BorderBottom.ToString(t));
                if (null != BorderLeft && !BorderLeft.IsEmpty) a.AppendAttribute("border-left", BorderLeft.ToString(t));
                if (null != BorderRight && !BorderRight.IsEmpty) a.AppendAttribute("border-right", BorderRight.ToString(t));
            }            
            return a.ToString();
        }
    }

    public class ThemeBorder
    {
        public string Width { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }

        public bool IsEmpty
        {
            get { return String.IsNullOrEmpty(Width) ||
                         String.IsNullOrEmpty(Style) ||
                         String.IsNullOrEmpty(Color); }
        }

        public string ToString(Theme t)
        {
            return String.Format("{0} {1} {2}", Width, Style, Color);
        }

        public static bool AreSame(params ThemeBorder[] borders)
        {
            if (borders.Any(x => null == x || x.IsEmpty)) return false;
            for (var n=1; n<borders.Length; n++)
            {
                if (borders[0].Width != borders[n].Width ||
                    borders[0].Style != borders[n].Style ||
                    borders[0].Color != borders[n].Color)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class ThemeSpacing
    {
        public string Top { get; set; }
        public string Bottom { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }   

        public bool IsEmpty
        {
            get { return !(String.IsNullOrEmpty(Top) ||
                           String.IsNullOrEmpty(Bottom) ||
                           String.IsNullOrEmpty(Left) ||
                           String.IsNullOrEmpty(Right)); }
        }

        public bool VerticallyLocked
        {
            get { return String.IsNullOrEmpty(Top) &&
                         String.IsNullOrEmpty(Bottom) &&
                         Top == Bottom; }
        }

        public bool HorizontallyLocked
        {
            get { return String.IsNullOrEmpty(Left) &&
                         String.IsNullOrEmpty(Right) &&
                         Left == Right; }
        }

        public bool FullyLocked
        {
            get { return VerticallyLocked &&
                         HorizontallyLocked &&
                         Top == Left; }
        }

        public override string ToString()
        {   
            if (IsEmpty) return String.Empty;
            if (FullyLocked)
            {
                return Top;
            }
            if (VerticallyLocked && HorizontallyLocked)
            {
                return String.Format("{0} {1}", Top, Left);
            }
            return String.Format("{0} {1} {2} {3}", Top.Or("0"), Right.Or("0"), Bottom.Or("0"), Left.Or("0"));
        }
    }

    public class StylesheetStringBuilder
    {
        readonly StringBuilder _builder = new StringBuilder();

        public void AppendAttribute(string attribute, string value)
        {
            if (String.IsNullOrEmpty(value)) return;
            if (_builder.Length > 0) _builder.Append("\n    ");
            _builder.AppendFormat("{0}: {1};", attribute, value);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}