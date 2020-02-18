#pragma once
#include <sstream>
#include "../../SundryUtilty/Cpp code files/JanyeeDateTime/DateTime.h"

struct MainPageHelper
{
	static Janyee::DateTime RandomDateTime(Janyee::DateTime const& lowerLimitDate, Janyee::DateTime const& upperLimitDate) {
		auto lowerLimitYear = lowerLimitDate.GetYear();
		auto upperLimitYear = upperLimitDate.GetYear();
		auto lowerLimitMonth = lowerLimitDate.GetMonth();
		auto upperLimitMonth = upperLimitDate.GetMonth();
		auto lowerLimitDay = lowerLimitDate.GetDayOfMonth();
		auto upperLimitDay = upperLimitDate.GetDayOfMonth();
		decltype(upperLimitYear) year = RandomNumber(lowerLimitYear, upperLimitYear);
		decltype(upperLimitMonth) month = 0;
		decltype(upperLimitDay) day = 0;
		try {
			if (year == lowerLimitYear) {
				month = RandomNumber(lowerLimitMonth, 12);
				if (month == lowerLimitMonth) {
					day = RandomNumber(lowerLimitDay, CalculateUpperDay(year, month));
				}
				else {
					day = RandomNumber(1, CalculateUpperDay(year, month));
				}
			}
			else if (year == upperLimitYear) {
				month = RandomNumber(1, upperLimitMonth);
				if (month == upperLimitMonth) {
					day = RandomNumber(1, upperLimitDay);
				}
				else {
					day = RandomNumber(1, CalculateUpperDay(year, month));
				}
			}
			else {
				month = RandomNumber(1, 12);
				day = RandomNumber(1, CalculateUpperDay(year, month));
			}
			return Janyee::DateTime(year, month, day);
		}
		catch (const std::out_of_range & e) {
			std::stringstream s;
			s << "CalculateUpperDay raise out of range exception. Year value is "
				<< std::to_string(year)
				<< ". Month value is "
				<< std::to_string(month)
				<< ". Day value is "
				<< std::to_string(day)
				<< "\n";
			s << e.what() << "\n";
			OutputDebugStringA(s.str().c_str());
			throw std::out_of_range(s.str());
		}
	}

	static double RandomEventFrequency(double downLimit, double upperLimit) {
		return RandomNumber(downLimit, upperLimit);
	}

	static auto const& RandomData() {
		thread_local static std::array<typename std::mt19937::result_type, std::mt19937::state_size> data;
		thread_local static std::random_device rd;

		std::generate(std::begin(data), std::end(data), std::ref(rd));

		return data;
	}

	static std::mt19937& RandomGenerator() {
		auto const& data = RandomData();

		thread_local static std::seed_seq seeds(std::begin(data), std::end(data));
		thread_local static std::mt19937 gen { seeds };

		return gen;
	}

	template<typename T> static T RandomNumber(T from, T to) {
		using Distribution = typename std::conditional
			<
			std::is_integral<T>::value,
			std::uniform_int_distribution<T>,
			std::uniform_real_distribution<T>
			>::type;

		thread_local static Distribution dist;

		return dist(RandomGenerator(), typename Distribution::param_type { from, to });
	}

private:
	// 返回指定月份的天数上限
	static int CalculateUpperDay(int& year, int& month) {
		std::map<int, int> monthPair;
		monthPair[1] = 31;
		monthPair[2] = Janyee::DateTime::IsLeapYear(year) ? 29 : 28;
		monthPair[3] = 31;
		monthPair[4] = 30;
		monthPair[5] = 31;
		monthPair[6] = 30;
		monthPair[7] = 31;
		monthPair[8] = 31;
		monthPair[9] = 30;
		monthPair[10] = 31;
		monthPair[11] = 30;
		monthPair[12] = 31;
		return monthPair.at(month);
	}
};

