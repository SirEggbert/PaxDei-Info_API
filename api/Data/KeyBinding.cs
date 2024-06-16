namespace api.Data
{
    public class KeyBinding
    {
        public int id {  get; set; }
        public string Category { get; set; } = "";
        public string Binding { get; set; } = "";
        public string? Action { get; set; }
    }
}
