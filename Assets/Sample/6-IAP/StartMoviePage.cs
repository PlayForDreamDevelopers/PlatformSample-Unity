using UnityEngine;
using YVR.Platform;

public class StartMoviePage : OperationPage
{
    protected override void OnClickExecute()
    {
        base.OnClickExecute();

        StartMovie();
    }

    private void StartMovie()
    {
        string sku = GetInputValueByType(InputOptionType.ProductSKU);
            IAP.StartMovie(sku).OnComplete(StartMovieCallback);
    }

    private void StartMovieCallback(YVRMessage msg)
    {
        if (msg.isError)
        {
            Debug.LogError($"StartMoviePage.StartMovieCallback: error -> {msg.error}");
            m_TextResult.text = msg.error.ToString();
        }
        else
        {
            Debug.Log($"StartMoviePage.StartMovieCallback: message info -> {msg}");
            m_TextResult.text = msg.ToString();
        }
    }
}