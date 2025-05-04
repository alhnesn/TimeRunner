public interface IRewindable
{
    /// <summary>
    /// Called every physics tick in normal play. 
    /// Should snapshot whatever state you need.
    /// </summary>
    void RecordState(float timeStamp);

    /// <summary>
    /// Called every physics tick during rewind.
    /// Should restore your object to what it was at timeStamp.
    /// </summary>
    void RestoreState(float timeStamp);
}
