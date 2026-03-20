using UnityEngine;
using System.Collections.Generic;

public class ResponseSkillQueryEventArgs : ExtendedEventArgs {
    public short Status { get; set; }
    public int PlayerId { get; set; }
    public int SkillId { get; set; }
    public string SkillName { get; set; }

    public ResponseSkillQueryEventArgs() {
        Event_id = Constants.SMSG_SKILL_QUERY;
    }
}

public class ResponseSkillQuery : BaseNetworkResponse {
    private int playerId;
    private int skillId;
    private string skillName;

    public ResponseSkillQuery() {
        Response_id = Constants.SMSG_SKILL_QUERY;
    }

    protected override void ParseResponseData() {
        playerId = DataReader.ReadInt(DataStream);
        skillId = DataReader.ReadInt(DataStream);
        skillName = DataReader.ReadString(DataStream);
    }

    public override ExtendedEventArgs Process() {
        ResponseSkillQueryEventArgs args = new ResponseSkillQueryEventArgs();
        args.Status = status;
        args.PlayerId = playerId;
        args.SkillId = skillId;
        args.SkillName = skillName;
        return args;
    }
}
