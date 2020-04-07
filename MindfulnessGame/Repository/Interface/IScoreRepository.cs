using System;

namespace MindfulnessGame.Repository.Interface
{
    public interface IScoreRepository
    {
        void WriteResult(int score);

        int GetRecord();

        event Action<int> RecordChanged;
    }
}