using System.Diagnostics.Eventing.Reader;

namespace MVC.Models
{
    public class Summernote
    {
        public Summernote(string idEditor, bool loadLibraries)
        {
            IdEditor = idEditor;
            LoadLibraries = loadLibraries;
        }

        public string IdEditor { get; set; }
        public bool LoadLibraries { get; set; }
        public int Height { get; set; } = 120;
        public string ToolBar { get; set; } = @"[
                ['style', ['style']],
                ['font', ['bold', 'underline', 'clear']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link', 'picture', 'video', 'elfinder']],
                ['view', ['fullscreen', 'codeview', 'help']]
            ]";
    }
}
