using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum ButtonTargetEnum { Layer, self, newwindow }

    [HtmlTargetElement("wt:linkbutton", TagStructure = TagStructure.WithoutEndTag)]
    public class LinkButtonTagHelper : BaseButtonTag
    {
        /// <summary>
        /// 是否使用<a>的形式输出
        /// </summary>
        public bool IsLink { get; set; }

        /// <summary>
        /// 跳转模式
        /// </summary>
        public ButtonTargetEnum? Target { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 如果跳转模式是Layer，设置弹出窗口标题
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// 如果跳转模式是Layer，设置弹出窗口宽度
        /// </summary>
        public int? WindowWidth { get; set; }

        /// <summary>
        /// 如果跳转模式是Layer，设置弹出窗口高度
        /// </summary>
        public int? WindowHeight { get; set; }

        /// <summary>
        /// 如果按钮在表单内，则使用表单数据Post到Url中
        /// </summary>
        public bool PostCurrentForm { get; set; }

        public bool Max { get; set; }

        /// <summary>
        /// 确认内容（如果不为空则弹出询问框）
        /// </summary>
        public string ConfirmTxt { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsLink == false)
            {
                output.Attributes.SetAttribute("type", "button");
            }
            else
            {
                output.TagName = "a";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.SetAttribute("href", "#");
            }
            if (Target == null || Target == ButtonTargetEnum.Layer)
            {
                string windowid = Guid.NewGuid().ToString();
                if (PostCurrentForm == true && context.Items.ContainsKey("formid"))
                {
                    Click = $"ff.OpenDialog('{Url}', '{windowid}', '{WindowTitle ?? ""}',{WindowWidth?.ToString() ?? "null"}, {WindowHeight?.ToString() ?? "null"}, ff.GetPostData('{context.Items["formid"]}'),{Max.ToString().ToLower()})";
                }
                else
                {
                    Click = $"ff.OpenDialog('{Url}', '{windowid}', '{WindowTitle ?? ""}',{WindowWidth?.ToString() ?? "null"}, {WindowHeight?.ToString() ?? "null"},undefined,{Max.ToString().ToLower()})";
                }
            }
            else if (Target == ButtonTargetEnum.self)
            {
                if (PostCurrentForm == true && context.Items.ContainsKey("formid"))
                {
                    Click = $"ff.BgRequest('{Url}',ff.GetPostData('{context.Items["formid"]}'))";
                }
                else
                {
                    Click = $"ff.BgRequest('{Url}')";
                }
            }
            else if (Target == ButtonTargetEnum.newwindow)
            {
                if (Url.StartsWith("~"))
                {
                    Url = Url.TrimStart('~');
                    Click = $"ff.SetCookie('#{Url}','{WindowTitle ?? ""}',true);window.open('{Url}')";
                }
                else
                {
                    Click = $"ff.SetCookie('#{Url}','{WindowTitle ?? ""}',true);window.open('/Home/PIndex#{Url}')";
                }
            }
            if (!string.IsNullOrEmpty(ConfirmTxt))
            {
                Click = $"layer.confirm('{ConfirmTxt}', {{icon: 3, title:'提示'}}, function(index){{ {Click};layer.close(index); }})";
            }
            base.Process(context, output);
        }

    }
}
