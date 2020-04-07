using System;
using MindfulnessGame.Repository.Interface;

namespace MindfulnessGame.Repository
{
    public class ScoreRepository : IScoreRepository
    {
        private int _record;

        public ScoreRepository()
        {
            Record = 17; //Как будто бы результат получили из базы
        }

        public int Record
        {
            get => _record;
            set
            {
                _record = value;
                RecordChanged?.Invoke(_record);
            }
        }

        public void WriteResult(int score)
        {
            if (score > _record)
                Record = score;
        }

        public int GetRecord()
        {
            return _record;
        }

        public event Action<int> RecordChanged;
    }
}