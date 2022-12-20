namespace BoxSampleAutoCAD.BoxIntegration.DataModels
{
    public interface IBoxAuthCredential
    {
        string ClientId { get; set; }
        string Secret { get; set; }
    }
}