#pragma once
#ifndef _SCORE_STRING_
#define __SCORESTRING__
#include "PluginSettings.h"
#include "Score.h"
#include <string>

class PLUGIN_API ScoreString {
public:
	ScoreString();
	Score GetScore() const;
	void SetScore(int score = 0);
	void SetLength(int length = 0);
	std::string ToString() const;

private:
	Score score;
};

#endif

