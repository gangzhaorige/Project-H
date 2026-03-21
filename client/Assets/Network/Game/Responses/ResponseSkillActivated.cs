using UnityEngine;

public class ResponseSkillActivatedEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int PlayerId { get; set; }
    public int SkillIndex { get; set; }

    public ResponseSkillActivatedEventArgs() {
        Event_id = Constants.SMSG_SKILL_ACTIVATION;
    }
}

public class ResponseSkillActivated : BaseNetworkResponse {
    private int playerId;
    private int skillIndex;

    public ResponseSkillActivated() {
        Response_id = Constants.SMSG_SKILL_ACTIVATION;
    }

    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        skillIndex = DataReader.ReadInt(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseSkillActivatedEventArgs args = new ResponseSkillActivatedEventArgs();
        args.Status = status;
        args.PlayerId = playerId;
        args.SkillIndex = skillIndex;
        return args;
    }
}
