// Gets the index of a sample in a curve that has been converted to an array of values
int GetSampleIndex(float value, int steps, float rangeStart, float rangeEnd)
{
    float rangeLength = rangeEnd - rangeStart;
    int index = round((rangeLength / (float) steps) * value * steps);
    index = index >= steps ? steps - 1 : index;
    return index;
}