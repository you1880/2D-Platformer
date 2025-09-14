using System.Collections;
using System.Collections.Generic;

public enum MessageID
{
    SaveData,
    DeleteSaveData,
    SaveDataError,
    SaveDataSuccess,
    LoadDataError,
    NoSaveWarning,
    GoTitle,
    StageFail,
}

public static class MessageTable
{
    private static readonly Dictionary<MessageID, string> _messageTable = new Dictionary<MessageID, string>
    {
        {MessageID.SaveData, "현재 데이터를 저장하시겠습니까?"},
        {MessageID.DeleteSaveData, "해당 세이브를 삭제하시겠습니까?"},
        {MessageID.SaveDataError, "데이터를 저장하는데 실패하였습니다."},
        {MessageID.SaveDataSuccess, "데이터가 성공적으로 저장되었습니다."},
        {MessageID.LoadDataError, "데이터를 불러오는데 실패하였습니다."},
        {MessageID.NoSaveWarning, "저장되지 않은 데이터는 사라집니다."},
        {MessageID.GoTitle, "저장되지 않은 데이터는 사라집니다. 타이틀 화면으로 이동하시겠습니까?"}, 
        {MessageID.StageFail, "스테이지 클리어에 실패하였습니다. 로비로 돌아갑니다."},
    };

    public static string GetMessage(MessageID id)
    {
        if (_messageTable.TryGetValue(id, out string msg))
        {
            return msg;
        }

        return "";
    }
}
