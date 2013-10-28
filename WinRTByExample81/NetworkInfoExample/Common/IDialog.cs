namespace NetworkInfoExample.Common
{
    using System.Threading.Tasks;

    public interface IDialog
    {
        Task ShowMessageAsync(string message);
    }
}