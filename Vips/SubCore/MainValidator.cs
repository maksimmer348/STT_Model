using System.Collections.ObjectModel;

namespace Vips;

public class MainValidator
{
    #region Vips

    //TODO дополнить потом
    private char[] invalidSymbols = new[] {'*', '@'};

    public bool ValidateInvalidSymbols(string str)
    {
        foreach (var t in invalidSymbols)
        {
            if (str.Contains(t))
            {
                return false;
            }
        }

        return true;
    }

    public bool ValidateCollisionName(string str, ObservableCollection<Vip> vips) => vips.All(v => v.Name != str);

    #endregion

    #region Devices

    public List<int> BusyPorts = new List<int>();

    
    #endregion

    public bool ValidateCollisionPort(int portNum)
    {
        if (BusyPorts.All(x => x != portNum))
        {
            return true;
        }

        return false;
    }
}