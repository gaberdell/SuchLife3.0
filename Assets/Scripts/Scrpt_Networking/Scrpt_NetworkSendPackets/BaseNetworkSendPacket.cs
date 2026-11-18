using PlasticGui.WorkspaceWindow;
using UnityEngine;

//Sorry for high documentation kind of felt like this needed it though
//Have had multiple instances of me getting confused on what packets
//did what so this is the solution.

/// <summary>
/// Abstract Class <c>BaseNetworkSendPacket</c> is an abstract class
/// created to allow for easy processing of various game objects
/// in <c>ClientNetworkManager</c> and <c>ServerNetworkManager</c>
/// through the use of abstraction.
/// </summary>
public abstract class BaseNetworkSendPacket
{
    public virtual bool IsTcp { get; protected set; }

    /// <summary>
    /// Bytes used when the constructor with bytes[] is called
    /// for processing a class in that constructor
    /// </summary>
    public int BytesUsed { get; protected set; } = -1;

    /// <summary>
    /// Processes the given data from the child classes constructor
    /// and returns bytes based on it. 
    /// <example>
    /// <para>
    /// For example:
    /// </para>
    /// <code>
    /// private bool isFoo
    /// public ChildClass(bool iF) {
    ///     isFoo = iF;
    /// }
    /// public override byte[] GetPacketBytes() {
    ///     if (isFoo) {
    ///         return new byte[1]{1};
    ///     }
    ///     return new byte[1]{0};
    /// }
    /// </code>
    /// </example>
    /// </summary>
    /// <returns>The bytes nessecary to be sent to 
    /// client/server</returns>
    public abstract byte[] GetPacketBytes();

    /// <summary>
    /// Execute the command associated with the network packet
    /// </summary>
    public abstract void ExecuteSentBytes();

    /// <summary>
    /// Processes bytes to preform some action on them delegated
    /// in a concrete child class
    /// </summary>
    /// <param name="sentBytes">Bytes sent from a udp/tcp 
    /// source that is potentially from a class such as this
    /// from the client/server</param>
    /// <returns>Returns true if the given bytes were
    /// intended to be processed by this false if not.</returns>
    //public abstract bool ProcessSentBytes(byte[] sentBytes);
}
