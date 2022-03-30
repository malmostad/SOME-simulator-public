using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoMeSimulator.Services.MessageManager.Dto
{
    public class Dialog
    {
        public Dialog(bool open, string title, string content, string confirmText)
        {
            Open = open;
            Title = title;
            Content = content;
            ConfirmText = confirmText;
        }

        [JsonProperty("open")]
        public bool Open { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("confirmText")]
        public string ConfirmText { get; set; }
    }
}
