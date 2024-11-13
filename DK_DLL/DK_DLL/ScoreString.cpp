#include "ScoreString.h"
#include <string>

ScoreString::ScoreString() {
	SetScore();
}

Score ScoreString::GetScore() const {
	return score;
}

void ScoreString::SetScore(int score) {
	this->score.score = score;
}

void ScoreString::SetLength(int length) {
	this->score.length = length;
}

std::string ScoreString::ToString() const {
	auto scoreString = std::to_string(score.score);
	if (score.length == 0) {
		return scoreString;
	}
	else {
		auto scoreLength = scoreString.length();
		if (scoreLength < score.length) {
			auto padding = std::string(score.length - scoreLength, '0');
			return padding + scoreString;
		}
		else {
			return scoreString;
		}
	}
}