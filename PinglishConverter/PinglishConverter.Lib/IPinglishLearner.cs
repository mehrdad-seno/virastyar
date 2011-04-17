//
// Author: Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Mehrdad Senobari at 2010-March-08
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCICT.NLP.Utility.PinglishConverter
{
    public interface IPinglishLearner
    {
        void Learn(PinglishString pinglishString);
        void Learn(List<PinglishString> listOfWords);
    }
}
