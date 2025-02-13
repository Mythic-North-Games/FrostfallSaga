using System;
using UnityEngine;

namespace FrostfallSaga.Core.Dialogues
{
    [Serializable]
    public class DialogueLine
    {
        [field: SerializeField] public string Title { get; private set; }
        [field: SerializeField] public string RichText { get; private set; }
        [field: SerializeField] public DialogueParticipantSO Speaker { get; private set; }
        [field: SerializeField] public bool IsRight { get; private set; }
        [field: SerializeField] public string[] Answers { get; private set; }

        /// <summary>
        /// Constructor for DialogueLine.
        /// </summary>
        /// <param name="title">The dialogue line's title (displayed only in editor)</param>
        /// <param name="richText">The text the speaker says.</param>
        /// <param name="speaker">The speaker that says the line.</param>
        /// <param name="isRight">If the speaker is placed on the right side of the screen.</param>
        /// <param name="answers">Optional answers for this dialogue line.</param>
        public DialogueLine(string title, string richText, DialogueParticipantSO speaker, bool isRight, string[] answers = null)
        {
            Title = title;
            RichText = richText;
            Speaker = speaker;
            IsRight = isRight;
            Answers = answers;
            if (answers == null)
            {
                Answers = Array.Empty<string>();
            }
        }

        public void SetTitle(string newTitle)
        {
            Title = newTitle;
        }

        public void SetRichText(string newRichText)
        {
            RichText = newRichText;
        }

        public void SetSpeaker(DialogueParticipantSO newSpeaker)
        {
            Speaker = newSpeaker;
        }

        public void SetIsRight(bool newIsRight)
        {
            IsRight = newIsRight;
        }

        public void SetAnswers(string[] newAnswers)
        {
            Answers = newAnswers;
        }
    }
}