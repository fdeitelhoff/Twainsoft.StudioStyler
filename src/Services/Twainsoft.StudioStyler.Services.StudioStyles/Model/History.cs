namespace Twainsoft.StudioStyler.Services.StudioStyles.Model
{
    public class History
    {
        public Scheme Scheme { get; private set; }
        public int Activations { get; set; }

        public History(Scheme scheme)
        {
            Scheme = scheme;
            Activations = 1;
        }
    }
}
