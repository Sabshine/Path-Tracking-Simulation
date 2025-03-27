using MathNet.Numerics.LinearAlgebra;

//source: https://github.com/degagawolde/Kalman-filter-Implementation-using-C-Sharp
public class KalmanFilter
{
    private Matrix<double> transitionMatrix;
    private Matrix<double> measurementMatrix;
    private Matrix<double> processNoise;
    private Matrix<double> measurementNoise;
    private Matrix<double> IdentityMatrix;
    public KalmanFilter()
    {
        transitionMatrix = Matrix<double>.Build.DenseIdentity(2);
        measurementMatrix = Matrix<double>.Build.DenseIdentity(2); ;

        processNoise = Matrix<double>.Build.Diagonal(2, 2, 1.0e-4);
        measurementNoise = Matrix<double>.Build.Diagonal(2, 2, 1.0e-1);
        IdentityMatrix = Matrix<double>.Build.DenseIdentity(2);
    }
    public void ApplyKalmanFilter(Matrix<double> measurement, ref Matrix<double> state, ref Matrix<double> errorCovariancePost)
    {

        //predict
        state = transitionMatrix.Multiply(state);
        errorCovariancePost = transitionMatrix.Multiply(errorCovariancePost).Multiply(transitionMatrix.Transpose()) + processNoise;

        //update
        Matrix<double> S = (measurementMatrix.Multiply(errorCovariancePost)).Multiply(measurementMatrix.Transpose()) + measurementNoise;
        Matrix<double> K = (errorCovariancePost.Multiply(measurementMatrix.Transpose())).Multiply(S.Inverse());
        state = state + K.Multiply(measurement - measurementMatrix.Multiply(state));
        errorCovariancePost = (IdentityMatrix - (K.Multiply(measurementMatrix)).Multiply(errorCovariancePost));
    }
}