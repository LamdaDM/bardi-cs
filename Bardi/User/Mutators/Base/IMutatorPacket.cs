namespace Bardi.User.Mutators.Base;

public interface IMutatorPacket
{
    public int Idx { get; }
    public Asset Target { get; }
    decimal Change { get; }
    bool IsAmountAdd { get; }
    public int Cycle { get; }
    public decimal TotalChange { get; }
    public long ReferenceDateDays { get; }
}