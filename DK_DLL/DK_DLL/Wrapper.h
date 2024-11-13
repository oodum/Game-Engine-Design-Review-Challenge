#pragma once
#ifndef __WRAPPER__
#define __WRAPPER__
#include "Score.h"
#include "PluginSettings.h"
#include <string>
#ifdef __cplusplus
extern "C" {
#endif
	PLUGIN_API Score GetScore();
	PLUGIN_API void SetScore(int score);
	PLUGIN_API void SetLength(int length);
	PLUGIN_API std::string ToString();
#ifdef __cplusplus
}
#endif
#endif
