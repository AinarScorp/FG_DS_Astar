using System;

[Serializable]
struct DataToSerialise<TElement>
{
    public int Index0;
    public int Index1;
    public TElement Element;
    public DataToSerialise(int idx0, int idx1, TElement element)
    {
        Index0 = idx0;
        Index1 = idx1;
        Element = element;
    }
}
