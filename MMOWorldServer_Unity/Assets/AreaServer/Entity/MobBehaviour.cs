namespace Assets.AreaServer.Entity
{
    internal enum MobBehaviour
    {
        Agressive,  //Attack if detects
        Passive,    //Attack if attacked
        Shy,        //Run if attacked
        Friendly    //NPC able to speak with
    }
}