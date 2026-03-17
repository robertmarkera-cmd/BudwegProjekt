using System.Drawing;

public class Caliper
{
    public string? Type { get; set; }
    public string? Manufacturer { get; set; }
    public string? Comment { get; set; }
    public Bitmap? Picture { get; set; }
    public int FrameID { get; set; }
    public int BatchID { get; set; }   // ← VIGTIG!
}