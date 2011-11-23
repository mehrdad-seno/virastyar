//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//

using System.Collections.Generic;

namespace SCICT.NLP.Utility.Transliteration.KNN
{
    public interface IPinglishLearner
    {
        void Learn(PinglishString pinglishString);
        void Learn(List<PinglishString> listOfWords);
    }
}
